using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Connection;
using RabbitMQ.Event;

namespace IoCmanage
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
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
