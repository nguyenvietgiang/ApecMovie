using FluentValidation;
using FluentValidation.AspNetCore;
using Minio;
using MovieServices.Application.BussinessServices;
using MovieServices.Application.Mapping;
using MovieServices.Application.ModelsDTO;
using MovieServices.Application.Validator;
using MovieServices.Domain.Interfaces;
using MovieServices.Infrastructure.Context;
using MovieServices.Infrastructure.Repository;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog;
using System.Reflection;
using ApecMovieCore.Middlewares;
using Microsoft.OpenApi.Models;
using RabbitMQ.Connection;
using RabbitMQ.Event;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ApecCoreIdentity;

var builder = WebApplication.CreateBuilder(args);
// logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
    .Enrich.FromLogContext()
    .WriteTo.File(new JsonFormatter(), "logs/log.txt", rollingInterval: RollingInterval.Day)
    // .WriteTo.Sink(new MongoDBSink(database, "LogEntries"))
    .CreateLogger();

// Add services to the container.

// thêm để add validator
builder.Services.AddControllers()
        .AddFluentValidation(fv => fv.ImplicitlyValidateChildProperties = true);

builder.Logging.AddSerilog();
builder.Services.AddLogging();



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = JwtConfig.Issuer, 
            ValidateAudience = true,
            ValidAudience = JwtConfig.Audience, 
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtConfig.SecretKey)) 
        };
    });

builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "1.0",
        Title = "APEC Movie Services",
        Description = "Movie Services for APEC Backend Microservices"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});



var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

// message broker
builder.Services.AddSingleton<IRabbitmqConnection>(new RabbitmqConnection());
builder.Services.AddScoped<IMessageProducer, RabbitmqProducer>();

// cấu hình minio
var minioConfig = configuration.GetSection("Minio");
var endpoint = minioConfig["Endpoint"];
var accessKey = minioConfig["AccessKey"];
var secretKey = minioConfig["SecretKey"];
var bucketName = minioConfig["BucketName"];


var minioClient = new MinioClient()
    .WithEndpoint(endpoint)
    .WithCredentials(accessKey, secretKey)
    .Build();

builder.Services.AddSingleton(minioClient);


builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddTransient<IValidator<MovieDTO>, MovieDTOValidator>();

builder.Services.AddDbContext<MovieDbConext>();

// Repository
builder.Services.AddScoped<IMovieRepository, MovieRepository>();

//Services
builder.Services.AddScoped<IMovieServices, MovieServicesImplementation>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseErrorHandlingMiddleware();

app.UseHttpsRedirection();

app.UseAuthentication(); // Thêm middleware xác thực
app.Use(async (context, next) =>
{
    var middleware = new JwtMiddleware();
    await middleware.Invoke(context, next);
});

app.UseAuthorization();

app.MapControllers();

app.Run();


