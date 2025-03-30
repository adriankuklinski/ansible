using Ansible.Infrastructure.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ansible.API.Health;

public class DatabaseHealthCheck : IHealthCheck
{
    private readonly AnsibleDbContext _dbContext;

    public DatabaseHealthCheck(AnsibleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Try to connect to the database
            var canConnect = await _dbContext.Database.CanConnectAsync(cancellationToken);

            if (canConnect)
            {
                return HealthCheckResult.Healthy("Database connection is healthy.");
            }
            else
            {
                return HealthCheckResult.Unhealthy("Cannot connect to the database.");
            }
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database health check failed.", ex);
        }
    }
}