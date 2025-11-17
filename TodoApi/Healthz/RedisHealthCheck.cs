using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TodoApi.Healthz
{
    public class RedisHealthCheck : IHealthCheck
    {
        private readonly IDistributedCache _cache;

        public RedisHealthCheck(IDistributedCache cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                // Try a simple set & get round-trip
                var testKey = $"healthz:{Guid.NewGuid()}";
                var testValue = "healthy";
                await _cache.SetStringAsync(testKey, testValue, cancellationToken);
                var result = await _cache.GetStringAsync(testKey, cancellationToken);
                if (result == testValue)
                {
                    await _cache.RemoveAsync(testKey, cancellationToken);
                    return HealthCheckResult.Healthy("Redis is healthy.");
                }
                return HealthCheckResult.Unhealthy("Redis did not return the expected value.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Redis is unhealthy.", ex);
            }
        }
    }
}
