using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Connection;
using RabbitMQ.Event;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog;

namespace IoCmanage
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            // cấu hình để logging chung
     Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
    .Enrich.FromLogContext()
    .WriteTo.File(new JsonFormatter(), "logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddSerilog();
            });


            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

            // cấu hình CROS
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

            // Đăng ký FluentValidation
            services.AddControllers()
                .AddFluentValidation(fv => fv.ImplicitlyValidateChildProperties = true);

            // Đăng ký RabbitMQ
            services.AddSingleton<IRabbitmqConnection>(new RabbitmqConnection());
            services.AddScoped<IMessageProducer, RabbitmqProducer>();

            return services;
        }
    }
}
