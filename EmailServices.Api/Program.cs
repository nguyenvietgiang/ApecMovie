using EmailServices.Api.Services;
using System.Reflection;
using Hangfire;
using Hangfire.MemoryStorage;
using ApecMovieCore.Middlewares;
using Microsoft.OpenApi.Models;
using RabbitMQ.Connection;
using RabbitMQ.Event;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add Swagger
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    // Set Swagger document properties
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "1.0",
        Title = "APEC Email Services",
        Description = "Email Services for APEC Backend Microservices"
    });
});

// Add Hangfire
builder.Services.AddHangfire(configuration => configuration.UseMemoryStorage());

// Message broker
builder.Services.AddSingleton<IRabbitmqConnection>(new RabbitmqConnection());
builder.Services.AddScoped<IMessageProducer, RabbitmqProducer>();
builder.Services.AddHostedService<ConsumerService>();
builder.Services.AddSingleton<IEmailService, EmailService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseErrorHandlingMiddleware();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.UseHangfireServer();
app.UseHangfireDashboard("/hangfire");


app.Run();