﻿using FreeSql;
using IGeekFan.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// <see cref="IHealthChecksBuilder"/> extension methods for Entity Framework Core.
/// </summary>
public static class FreeSqlHealthChecksBuilderExtensions
{
    /// <summary>
    /// Adds a health check for the specified <see cref="DbContext"/> type.
    /// </summary>
    /// <typeparam name="TContext">The <see cref="DbContext"/> type.</typeparam>
    /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
    /// <param name="name">
    /// The health check name. Optional. If <c>null</c> the type name of <typeparamref name="TContext"/> will be used for the name.
    /// </param>
    /// <param name="failureStatus">
    /// The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c> then
    /// the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.
    /// </param>
    /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
    /// <param name="customTestQuery">
    /// A custom test query that will be executed when the health check executes to test the health of the database
    /// connection and configurations.
    /// </param>
    /// <returns>The <see cref="IHealthChecksBuilder"/>.</returns>
    /// <remarks>
    /// <para>
    /// The health check implementation added by this method will use the dependency injection container
    /// to create an instance of <typeparamref name="TContext"/>.
    /// </para>
    /// <para>
    /// By default the health check implementation will use the <see cref="IAdo.ExecuteConnectTestAsync(int, CancellationToken)"/> method
    /// to test connectivity to the database. This method requires that the database provider has correctly implemented the
    /// <see cref="IAdo" /> interface. If the database provide has not implemented this interface
    /// then the health check will report a failure.
    /// </para>
    /// <para>
    /// Providing a <paramref name="customTestQuery" /> will replace the use of <see cref="IAdo.ExecuteConnectTestAsync(int, CancellationToken)"/>
    /// to test database connectivity. An implementation of a test query should handle exceptions that can arise due to connectivity failure,
    /// and should return a pass/fail result. The test query should be be designed to complete in a short and predicatable amount of time.
    /// </para>
    /// </remarks>
    public static IHealthChecksBuilder AddDbContextCheck<TContext>(
      this IHealthChecksBuilder builder,
      string? name = null,
      HealthStatus? failureStatus = default,
      IEnumerable<string>? tags = default,
      Func<TContext, CancellationToken, Task<bool>>? customTestQuery = default)
      where TContext : DbContext
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        if (name == null)
        {
            name = typeof(TContext).Name;
        }

        if (customTestQuery != null)
        {
            builder.Services.Configure<DbContextHealthCheckOptions<TContext>>(name, options => options.CustomTestQuery = customTestQuery);
        }

        return builder.AddCheck<DbContextHealthCheck<TContext>>(name, failureStatus, tags ?? Enumerable.Empty<string>());
    }
}