// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using IGeekFan.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Diagnostics.HealthChecks;
public class DbContextHealthCheckTest
{
    // Just testing healthy here since it would be complicated to simulate a failure. All of that logic lives in EF anyway.
    [Fact]
    public async Task CheckAsync_DefaultTest_Healthy()
    {
        // Arrange
        var services = CreateServices();
        using (var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var registration = Assert.Single(services.GetRequiredService<IOptions<HealthCheckServiceOptions>>().Value.Registrations);
            var check = ActivatorUtilities.CreateInstance<DbContextHealthCheck<TestDbContext>>(scope.ServiceProvider);

            // Act
            var result = await check.CheckHealthAsync(new HealthCheckContext() { Registration = registration, });

            // Assert
            Assert.Equal(HealthStatus.Healthy, result.Status);
        }
    }

    [Fact]
    public async Task CheckAsync_CustomTest_Healthy()
    {
        // Arrange
        var services = CreateServices(async (c, ct) =>
        {
            return await c.Blogs.Select.AnyAsync();
        });

        using (var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var registration = Assert.Single(services.GetRequiredService<IOptions<HealthCheckServiceOptions>>().Value.Registrations);
            var check = ActivatorUtilities.CreateInstance<DbContextHealthCheck<TestDbContext>>(scope.ServiceProvider);

            // Add a blog so that the custom test passes
            var context = scope.ServiceProvider.GetRequiredService<TestDbContext>();
            context.Add(new Blog());
            await context.SaveChangesAsync();

            // Act
            var result = await check.CheckHealthAsync(new HealthCheckContext() { Registration = registration, });

            // Assert
            Assert.Equal(HealthStatus.Healthy, result.Status);
        }
    }

    [Fact]
    public async Task CheckAsync_CustomTestWithDegradedFailureStatusSpecified_Degraded()
    {
        // Arrange
        var services = CreateServices(async (c, ct) =>
        {
            return await c.Blogs.Select.AnyAsync();
        }, failureStatus: HealthStatus.Degraded);

        using (var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var registration = Assert.Single(services.GetRequiredService<IOptions<HealthCheckServiceOptions>>().Value.Registrations);
            var check = ActivatorUtilities.CreateInstance<DbContextHealthCheck<TestDbContext>>(scope.ServiceProvider);

            // Act
            var result = await check.CheckHealthAsync(new HealthCheckContext() { Registration = registration, });

            // Assert
            Assert.Equal(HealthStatus.Degraded, result.Status);
        }
    }

    [Fact]
    public async Task CheckAsync_CustomTestWithUnhealthyFailureStatusSpecified_Unhealthy()
    {
        // Arrange
        var services = CreateServices(async (c, ct) =>
        {
            return await c.Blogs.Select.AnyAsync(ct);
        }, failureStatus: HealthStatus.Unhealthy);

        using (var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var registration = Assert.Single(services.GetRequiredService<IOptions<HealthCheckServiceOptions>>().Value.Registrations);
            var check = ActivatorUtilities.CreateInstance<DbContextHealthCheck<TestDbContext>>(scope.ServiceProvider);

            // Act
            var result = await check.CheckHealthAsync(new HealthCheckContext() { Registration = registration, });

            // Assert
            Assert.Equal(HealthStatus.Unhealthy, result.Status);
        }
    }

    [Fact]
    public async Task CheckAsync_CustomTestWithNoFailureStatusSpecified_Unhealthy()
    {
        // Arrange
        var services = CreateServices(async (c, ct) =>
        {
            return await c.Blogs.Select.AnyAsync();
        }, failureStatus: null);

        using (var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var registration = Assert.Single(services.GetRequiredService<IOptions<HealthCheckServiceOptions>>().Value.Registrations);
            var check = ActivatorUtilities.CreateInstance<DbContextHealthCheck<TestDbContext>>(scope.ServiceProvider);

            // Act
            var result = await check.CheckHealthAsync(new HealthCheckContext() { Registration = registration, });

            // Assert
            Assert.Equal(HealthStatus.Unhealthy, result.Status);
        }
    }

    private static IServiceProvider CreateServices(
        Func<TestDbContext, CancellationToken, Task<bool>> testQuery = null,
        HealthStatus? failureStatus = HealthStatus.Unhealthy)
    {
        var serviceCollection = DependencyInjection.FreeUtil.GetFreeSqlServiceCollection<TestDbContext>();

        var builder = serviceCollection.AddHealthChecks();
        builder.AddDbContextCheck("test", failureStatus, new[] { "tag1", "tag2", }, testQuery);
        return serviceCollection.BuildServiceProvider();
    }
}
