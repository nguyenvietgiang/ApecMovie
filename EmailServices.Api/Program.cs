using EmailServices.Api.Services;
using System.Reflection;
using Hangfire;
using Hangfire.MemoryStorage;
using ApecMovieCore.Middlewares;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    // Đặt tên và mô tả cho tài liệu Swagger
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "1.0",
        Title = "APEC Email Services",
        Description = "Email Services for APEC Backend Microservices"
    });
});

builder.Services.AddHangfire(configuration => configuration.UseMemoryStorage());

builder.Services.AddScoped<IEmailService, EmailService>();

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
app.UseHangfireDashboard("/hangfile");
app.Run();
