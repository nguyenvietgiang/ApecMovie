using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Connection;
using RabbitMQ.Event;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;

namespace IoCmanage
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services, string collectionName)
        {
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                .Enrich.FromLogContext()
                .WriteTo.File(new JsonFormatter(), "logs/log.txt", rollingInterval: Serilog.RollingInterval.Day)
                .WriteTo.MongoDB("mongodb://localhost:27017/ApecMovielogs", collectionName: collectionName)
                .CreateLogger();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddSerilog();
            });

            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

            // Configure CORS
            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                    policy =>
                    {
                        policy.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
            });

            // Register FluentValidation
            services.AddControllers()
                .AddFluentValidation(fv => fv.ImplicitlyValidateChildProperties = true);

            // Register RabbitMQ
            services.AddSingleton<IRabbitmqConnection>(new RabbitmqConnection());
            services.AddScoped<IMessageProducer, RabbitmqProducer>();

            return services;
        }
    }
}

