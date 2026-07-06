using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using IGeekFan.AspNetCore.SignalR.FreeRedis.Internal;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Internal;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RedisProtocol = IGeekFan.AspNetCore.SignalR.FreeRedis.Internal.RedisProtocol;

namespace IGeekFan.AspNetCore.SignalR.FreeRedis;

/// <summary>
/// The FreeRedis scale-out provider for multi-server SignalR support.
/// </summary>
public sealed class FreeRedisHubLifetimeManager<THub> : HubLifetimeManager<THub>, IDisposable where THub : Hub
{
    private readonly HubConnectionStore _connections = new();
    private readonly RedisSubscriptionManager _groups = new();
    private readonly RedisSubscriptionManager _users = new();
    private readonly ConcurrentDictionary<string, IDisposable> _subscriptions = new(StringComparer.Ordinal);
    private readonly ILogger _logger;
    private readonly FreeRedisOptions _options;
    private readonly RedisChannels _channels;
    private readonly string _serverName = GenerateServerName();
    private readonly RedisProtocol _protocol;
    private readonly SemaphoreSlim _connectionLock = new(1);
    private readonly IHubProtocolResolver _hubProtocolResolver;
    private readonly Microsoft.AspNetCore.SignalR.Internal.ClientResultsManager _clientResultsManager = new();
    private readonly AckHandler _ackHandler = new();
    private global::FreeRedis.IRedisClient? _redisClient;
    private bool _ownsRedisClient;
    private int _internalAckId;

    public FreeRedisHubLifetimeManager(
        ILogger<FreeRedisHubLifetimeManager<THub>> logger,
        IOptions<FreeRedisOptions> options,
        IHubProtocolResolver hubProtocolResolver)
        : this(logger, options, hubProtocolResolver, null, null)
    {
    }

    public FreeRedisHubLifetimeManager(
        ILogger<FreeRedisHubLifetimeManager<THub>> logger,
        IOptions<FreeRedisOptions> options,
        IHubProtocolResolver hubProtocolResolver,
        IOptions<HubOptions>? globalHubOptions,
        IOptions<HubOptions<THub>>? hubOptions)
    {
        _logger = logger;
        _options = options.Value;
        _hubProtocolResolver = hubProtocolResolver;
        _channels = new RedisChannels(typeof(THub).FullName!, _serverName);
        _protocol = new RedisProtocol(new DefaultHubMessageSerializer(
            hubProtocolResolver,
            globalHubOptions?.Value.SupportedProtocols,
            hubOptions?.Value.SupportedProtocols));
    }

    public override async Task OnConnectedAsync(HubConnectionContext connection)
    {
        await EnsureRedisServerConnection();

        _connections.Add(connection);
        connection.Features.Set<IRedisFeature>(new RedisFeature());

        var connectionTask = SubscribeToConnection(connection);
        Task userTask = Task.CompletedTask;
        if (!string.IsNullOrEmpty(connection.UserIdentifier))
        {
            userTask = SubscribeToUser(connection, connection.UserIdentifier);
        }

        await Task.WhenAll(connectionTask, userTask);
    }

    public override Task OnDisconnectedAsync(HubConnectionContext connection)
    {
        _connections.Remove(connection);

        var tasks = new List<Task>();
        var connectionChannel = _channels.Connection(connection.ConnectionId);
        RedisLog.Unsubscribe(_logger, connectionChannel);
        Unsubscribe(connectionChannel);

        var feature = connection.Features.GetRequiredFeature<IRedisFeature>();
        foreach (var group in feature.Groups)
        {
            tasks.Add(RemoveGroupAsyncCore(connection, group));
        }

        if (!string.IsNullOrEmpty(connection.UserIdentifier))
        {
            tasks.Add(RemoveUserAsync(connection));
        }

        return Task.WhenAll(tasks);
    }

    public override Task SendAllAsync(string methodName, object?[] args, CancellationToken cancellationToken = default)
    {
        var message = _protocol.WriteInvocation(methodName, args);
        return PublishAsync(_channels.All, message);
    }

    public override Task SendAllExceptAsync(string methodName, object?[] args, IReadOnlyList<string> excludedConnectionIds, CancellationToken cancellationToken = default)
    {
        var message = _protocol.WriteInvocation(methodName, args, excludedConnectionIds: excludedConnectionIds);
        return PublishAsync(_channels.All, message);
    }

    public override Task SendConnectionAsync(string connectionId, string methodName, object?[] args, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(connectionId);

        var connection = _connections[connectionId];
        if (connection is not null)
        {
            return connection.WriteAsync(new InvocationMessage(methodName, args), cancellationToken).AsTask();
        }

        var message = _protocol.WriteInvocation(methodName, args);
        return PublishAsync(_channels.Connection(connectionId), message);
    }

    public override Task SendConnectionsAsync(IReadOnlyList<string> connectionIds, string methodName, object?[] args, CancellationToken cancellationToken = default)
    {
        var tasks = new List<Task>(connectionIds.Count);
        var payload = _protocol.WriteInvocation(methodName, args);
        foreach (var connectionId in connectionIds)
        {
            tasks.Add(PublishAsync(_channels.Connection(connectionId), payload));
        }

        return Task.WhenAll(tasks);
    }

    public override Task SendGroupAsync(string groupName, string methodName, object?[] args, CancellationToken cancellationToken = default)
    {
        var message = _protocol.WriteInvocation(methodName, args);
        return PublishAsync(_channels.Group(groupName), message);
    }

    public override Task SendGroupExceptAsync(string groupName, string methodName, object?[] args, IReadOnlyList<string> excludedConnectionIds, CancellationToken cancellationToken = default)
    {
        var message = _protocol.WriteInvocation(methodName, args, excludedConnectionIds: excludedConnectionIds);
        return PublishAsync(_channels.Group(groupName), message);
    }

    public override Task SendGroupsAsync(IReadOnlyList<string> groupNames, string methodName, object?[] args, CancellationToken cancellationToken = default)
    {
        var tasks = new List<Task>(groupNames.Count);
        var payload = _protocol.WriteInvocation(methodName, args);
        foreach (var groupName in groupNames)
        {
            if (!string.IsNullOrEmpty(groupName))
            {
                tasks.Add(PublishAsync(_channels.Group(groupName), payload));
            }
        }

        return Task.WhenAll(tasks);
    }

    public override Task SendUserAsync(string userId, string methodName, object?[] args, CancellationToken cancellationToken = default)
    {
        var message = _protocol.WriteInvocation(methodName, args);
        return PublishAsync(_channels.User(userId), message);
    }

    public override Task SendUsersAsync(IReadOnlyList<string> userIds, string methodName, object?[] args, CancellationToken cancellationToken = default)
    {
        var tasks = new List<Task>(userIds.Count);
        var payload = _protocol.WriteInvocation(methodName, args);
        foreach (var userId in userIds)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                tasks.Add(PublishAsync(_channels.User(userId), payload));
            }
        }

        return Task.WhenAll(tasks);
    }

    public override Task AddToGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(connectionId);
        ArgumentNullException.ThrowIfNull(groupName);

        var connection = _connections[connectionId];
        return connection is not null
            ? AddGroupAsyncCore(connection, groupName)
            : SendGroupActionAndWaitForAck(connectionId, groupName, GroupAction.Add);
    }

    public override Task RemoveFromGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(connectionId);
        ArgumentNullException.ThrowIfNull(groupName);

        var connection = _connections[connectionId];
        return connection is not null
            ? RemoveGroupAsyncCore(connection, groupName)
            : SendGroupActionAndWaitForAck(connectionId, groupName, GroupAction.Remove);
    }

    public override async Task<T> InvokeConnectionAsync<T>(string connectionId, string methodName, object?[] args, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(connectionId);

        var connection = _connections[connectionId];
        var invocationId = GenerateInvocationId();
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, connection?.ConnectionAborted ?? default);
        var task = _clientResultsManager.AddInvocation<T>(connectionId, invocationId, cts.Token);

        try
        {
            if (connection is null)
            {
                var messageBytes = _protocol.WriteInvocation(methodName, args, invocationId, returnChannel: _channels.ReturnResults);
                var received = await PublishAsync(_channels.Connection(connectionId), messageBytes);
                if (received < 1)
                {
                    throw new IOException($"Connection '{connectionId}' does not exist.");
                }
            }
            else
            {
                await connection.WriteAsync(new InvocationMessage(invocationId, methodName, args), cancellationToken);
            }
        }
        catch
        {
            _clientResultsManager.RemoveInvocation(invocationId);
            throw;
        }

        try
        {
            return await task;
        }
        catch
        {
            if (connection?.ConnectionAborted.IsCancellationRequested == true)
            {
                throw new IOException($"Connection '{connectionId}' disconnected.");
            }

            throw;
        }
    }

    public override Task SetConnectionResultAsync(string connectionId, CompletionMessage result)
    {
        _clientResultsManager.TryCompleteResult(connectionId, result);
        return Task.CompletedTask;
    }

    public override bool TryGetReturnType(string invocationId, [NotNullWhen(true)] out Type? type)
    {
        return _clientResultsManager.TryGetType(invocationId, out type);
    }

    private async Task<long> PublishAsync(string channel, byte[] payload)
    {
        await EnsureRedisServerConnection();
        RedisLog.PublishToChannel(_logger, channel);
        return await _redisClient!.PublishAsync(channel, payload);
    }

    private Task AddGroupAsyncCore(HubConnectionContext connection, string groupName)
    {
        var feature = connection.Features.GetRequiredFeature<IRedisFeature>();
        var groupChannel = _channels.Group(groupName);

        if (feature.Groups.Add(groupName))
        {
            return _groups.AddSubscriptionAsync(groupChannel, connection, SubscribeToGroupAsync);
        }

        return Task.CompletedTask;
    }

    private Task RemoveGroupAsyncCore(HubConnectionContext connection, string groupName)
    {
        var feature = connection.Features.GetRequiredFeature<IRedisFeature>();
        if (!feature.Groups.Remove(groupName))
        {
            return Task.CompletedTask;
        }

        var groupChannel = _channels.Group(groupName);
        return _groups.RemoveSubscriptionAsync(groupChannel, connection, this, static (state, channelName) =>
        {
            var manager = (FreeRedisHubLifetimeManager<THub>)state;
            RedisLog.Unsubscribe(manager._logger, channelName);
            manager.Unsubscribe(channelName);
            return Task.CompletedTask;
        });
    }

    private Task RemoveUserAsync(HubConnectionContext connection)
    {
        var userChannel = _channels.User(connection.UserIdentifier!);
        return _users.RemoveSubscriptionAsync(userChannel, connection, this, static (state, channelName) =>
        {
            var manager = (FreeRedisHubLifetimeManager<THub>)state;
            RedisLog.Unsubscribe(manager._logger, channelName);
            manager.Unsubscribe(channelName);
            return Task.CompletedTask;
        });
    }

    private async Task SendGroupActionAndWaitForAck(string connectionId, string groupName, GroupAction action)
    {
        var id = Interlocked.Increment(ref _internalAckId);
        var ack = _ackHandler.CreateAck(id);
        var message = RedisProtocol.WriteGroupCommand(new RedisGroupCommand(id, _serverName, action, groupName, connectionId));
        await PublishAsync(_channels.GroupManagement, message);
        await ack;
    }

    private Task SubscribeToAll()
    {
        return SubscribeAsync(_channels.All, async payload =>
        {
            var invocation = RedisProtocol.ReadInvocation(payload);
            var tasks = new List<Task>(_connections.Count);
            foreach (var connection in _connections)
            {
                if (invocation.ExcludedConnectionIds is null || !invocation.ExcludedConnectionIds.Contains(connection.ConnectionId))
                {
                    tasks.Add(connection.WriteAsync(invocation.Message).AsTask());
                }
            }

            await Task.WhenAll(tasks);
        });
    }

    private Task SubscribeToGroupManagementChannel()
    {
        return SubscribeAsync(_channels.GroupManagement, async payload =>
        {
            var groupMessage = RedisProtocol.ReadGroupCommand(payload);
            var connection = _connections[groupMessage.ConnectionId];
            if (connection is null)
            {
                return;
            }

            if (groupMessage.Action == GroupAction.Remove)
            {
                await RemoveGroupAsyncCore(connection, groupMessage.GroupName);
            }
            else if (groupMessage.Action == GroupAction.Add)
            {
                await AddGroupAsyncCore(connection, groupMessage.GroupName);
            }

            await PublishAsync(_channels.Ack(groupMessage.ServerName), RedisProtocol.WriteAck(groupMessage.Id));
        });
    }

    private Task SubscribeToAckChannel()
    {
        return SubscribeAsync(_channels.Ack(_serverName), payload =>
        {
            _ackHandler.TriggerAck(RedisProtocol.ReadAck(payload));
            return Task.CompletedTask;
        });
    }

    private Task SubscribeToConnection(HubConnectionContext connection)
    {
        var connectionChannel = _channels.Connection(connection.ConnectionId);
        return SubscribeAsync(connectionChannel, async payload =>
        {
            var invocation = RedisProtocol.ReadInvocation(payload);

            if (!string.IsNullOrEmpty(invocation.InvocationId))
            {
                CancellationTokenRegistration? tokenRegistration = null;
                Action<object, CompletionMessage> complete = (_, completionMessage) =>
                {
                    tokenRegistration?.Dispose();
                    var protocolName = connection.Protocol.Name;
                    _ = Task.Run(async () =>
                    {
                        byte[] message;
                        try
                        {
                            var memoryBufferWriter = Microsoft.AspNetCore.Internal.MemoryBufferWriter.Get();
                            try
                            {
                                connection.Protocol.WriteMessage(completionMessage, memoryBufferWriter);
                                message = RedisProtocol.WriteCompletionMessage(memoryBufferWriter, protocolName);
                            }
                            finally
                            {
                                memoryBufferWriter.Dispose();
                            }

                            await PublishAsync(invocation.ReturnChannel!, message);
                        }
                        catch (Exception ex)
                        {
                            RedisLog.ErrorForwardingResult(_logger, completionMessage.InvocationId!, ex);
                        }
                    });
                };

                _clientResultsManager.AddInvocation(invocation.InvocationId,
                    (typeof(RawResult), connection.ConnectionId, new object(), complete));

                tokenRegistration = connection.ConnectionAborted.UnsafeRegister(_ =>
                {
                    var invocationInfo = _clientResultsManager.RemoveInvocation(invocation.InvocationId);
                    if (invocationInfo is { } info)
                    {
                        info.Completion(info.Tcs, CompletionMessage.WithError(invocation.InvocationId, "Connection disconnected."));
                    }
                }, null);
            }

            await connection.WriteAsync(invocation.Message);
        });
    }

    private Task SubscribeToUser(HubConnectionContext connection, string userIdentifier)
    {
        var userChannel = _channels.User(userIdentifier);
        return _users.AddSubscriptionAsync(userChannel, connection, (channelName, subscriptions) =>
            SubscribeAsync(channelName, async payload =>
            {
                var invocation = RedisProtocol.ReadInvocation(payload);
                var tasks = new List<Task>(subscriptions.Count);
                foreach (var userConnection in subscriptions)
                {
                    tasks.Add(userConnection.WriteAsync(invocation.Message).AsTask());
                }

                await Task.WhenAll(tasks);
            }));
    }

    private Task SubscribeToGroupAsync(string groupChannel, HubConnectionStore groupConnections)
    {
        return SubscribeAsync(groupChannel, async payload =>
        {
            var invocation = RedisProtocol.ReadInvocation(payload);
            var tasks = new List<Task>(groupConnections.Count);
            foreach (var groupConnection in groupConnections)
            {
                if (invocation.ExcludedConnectionIds?.Contains(groupConnection.ConnectionId) == true)
                {
                    continue;
                }

                tasks.Add(groupConnection.WriteAsync(invocation.Message).AsTask());
            }

            await Task.WhenAll(tasks);
        });
    }

    private Task SubscribeToReturnResultsAsync()
    {
        return SubscribeAsync(_channels.ReturnResults, payload =>
        {
            var completion = RedisProtocol.ReadCompletion(payload);
            var protocol = _hubProtocolResolver.AllProtocols.FirstOrDefault(p => p.Name.Equals(completion.ProtocolName, StringComparison.Ordinal));
            if (protocol is null)
            {
                RedisLog.MismatchedServers(_logger, completion.ProtocolName);
                return Task.CompletedTask;
            }

            var data = completion.CompletionMessage;
            HubMessage? hubMessage = null;
            var retryForError = false;
            try
            {
                retryForError = !protocol.TryParseMessage(ref data, _clientResultsManager, out hubMessage);
            }
            catch
            {
                retryForError = true;
            }

            if (retryForError)
            {
                try
                {
                    data = completion.CompletionMessage;
                    if (!protocol.TryParseMessage(ref data, FakeInvocationBinder.Instance, out hubMessage))
                    {
                        RedisLog.ErrorParsingResult(_logger, completion.ProtocolName, null);
                        return Task.CompletedTask;
                    }
                }
                catch (Exception ex)
                {
                    RedisLog.ErrorParsingResult(_logger, completion.ProtocolName, ex);
                    return Task.CompletedTask;
                }
            }

            var completionMessage = (CompletionMessage)hubMessage!;
            var invocationInfo = _clientResultsManager.RemoveInvocation(completionMessage.InvocationId!);
            if (retryForError && invocationInfo is not null)
            {
                completionMessage = CompletionMessage.WithError(
                    completionMessage.InvocationId!,
                    $"Client result wasn't deserializable to {invocationInfo.Value.Type.Name}.");
            }

            if (invocationInfo is { } info)
            {
                info.Completion(info.Tcs, completionMessage);
            }
            return Task.CompletedTask;
        });
    }

    private async Task SubscribeAsync(string channelName, Func<ReadOnlyMemory<byte>, Task> handler)
    {
        await EnsureRedisServerConnection();
        if (_subscriptions.ContainsKey(channelName))
        {
            return;
        }

        RedisLog.Subscribing(_logger, channelName);
        var subscription = _redisClient!.Subscribe(channelName, (redisChannel, message) =>
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    RedisLog.ReceivedFromChannel(_logger, channelName);
                    await handler(ToBytes(message));
                }
                catch (Exception ex)
                {
                    RedisLog.FailedWritingMessage(_logger, ex);
                }
            });
        });

        _subscriptions[channelName] = subscription;
    }

    private void Unsubscribe(string channelName)
    {
        if (_subscriptions.TryRemove(channelName, out var subscription))
        {
            subscription.Dispose();
        }
    }

    private async Task EnsureRedisServerConnection()
    {
        if (_redisClient is not null)
        {
            return;
        }

        await _connectionLock.WaitAsync();
        try
        {
            if (_redisClient is not null)
            {
                return;
            }

            RedisLog.ConnectingToRedis(_logger, _serverName);
            _redisClient = await _options.ConnectAsync(new LoggerTextWriter(_logger));
            _ownsRedisClient = _options.RedisClient is null;
            RedisLog.Connected(_logger);

            await SubscribeToAll();
            await SubscribeToGroupManagementChannel();
            await SubscribeToAckChannel();
            await SubscribeToReturnResultsAsync();
        }
        catch (Exception ex)
        {
            RedisLog.ConnectionFailed(_logger, ex);
            throw;
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    private static ReadOnlyMemory<byte> ToBytes(object? message)
    {
        return message switch
        {
            byte[] bytes => bytes,
            ReadOnlyMemory<byte> memory => memory,
            Memory<byte> memory => memory,
            string text => Convert.FromBase64String(text),
            _ => throw new InvalidOperationException($"Unsupported Redis message type '{message?.GetType().FullName}'.")
        };
    }

    private static string GenerateServerName()
    {
        return $"{Dns.GetHostName()}_{Guid.NewGuid():N}";
    }

    private static string GenerateInvocationId()
    {
        return Guid.NewGuid().ToString("N");
    }

    public void Dispose()
    {
        foreach (var subscription in _subscriptions.Values)
        {
            subscription.Dispose();
        }

        _subscriptions.Clear();
        _ackHandler.Dispose();

        if (_ownsRedisClient)
        {
            (_redisClient as IDisposable)?.Dispose();
        }
    }

    private sealed class FakeInvocationBinder : IInvocationBinder
    {
        public static readonly FakeInvocationBinder Instance = new();

        private FakeInvocationBinder()
        {
        }

        public IReadOnlyList<Type> GetParameterTypes(string methodName)
        {
            throw new NotImplementedException();
        }

        public Type GetReturnType(string invocationId)
        {
            return typeof(RawResult);
        }

        public Type GetStreamItemType(string streamId)
        {
            throw new NotImplementedException();
        }

        public string GetTarget(ReadOnlySpan<byte> utf8Bytes)
        {
            throw new NotImplementedException();
        }
    }

    private interface IRedisFeature
    {
        HashSet<string> Groups { get; }
    }

    private sealed class RedisFeature : IRedisFeature
    {
        public HashSet<string> Groups { get; } = new(StringComparer.OrdinalIgnoreCase);
    }

    private sealed class LoggerTextWriter : TextWriter
    {
        private readonly ILogger _logger;

        public LoggerTextWriter(ILogger logger)
        {
            _logger = logger;
        }

        public override Encoding Encoding => Encoding.UTF8;

        public override void WriteLine(string? value)
        {
            RedisLog.RedisConnectionMessage(_logger, value);
        }
    }
}
