using FreeSql;

namespace IGeekFan.Extensions.Diagnostics.HealthChecks;

internal sealed class DbContextHealthCheckOptions<TContext> where TContext : DbContext
{
    public Func<TContext, CancellationToken, Task<bool>>? CustomTestQuery { get; set; }
}