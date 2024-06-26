using Microsoft.EntityFrameworkCore;

namespace ApecFoodService.API.Extension
{
    public static class ApplicationBuilderExtensions
    {
        public static void AddAutoMigration<TContext>(this IApplicationBuilder app) where TContext : DbContext
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<TContext>();
                var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<TContext>>();

                try
                {
                    var pendingMigrations = context.Database.GetPendingMigrations();
                    if (pendingMigrations.Any())
                    {
                        logger.LogInformation("Applying pending migrations...");
                        context.Database.Migrate();
                        logger.LogInformation("Applied pending migrations successfully.");
                    }
                    else
                    {
                        logger.LogInformation("No pending migrations found.");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while applying migrations.");
                }
            }
        }
    }
}
