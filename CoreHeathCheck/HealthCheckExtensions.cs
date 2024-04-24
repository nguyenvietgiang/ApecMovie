using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;


namespace CoreHeathCheck
{
    public static class HealthCheckExtensions
    {
        public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services, string connection)
        {
            services.AddHealthChecks().AddNpgSql(connection);
            services.AddHealthChecks();
            services.AddHealthChecksUI().AddInMemoryStorage();
            return services;
        }

        public static IEndpointRouteBuilder MapCustomHealthChecks(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHealthChecks("/healthcheck");
            endpoints.MapHealthChecksUI();
            return endpoints;
        }
    }
}
