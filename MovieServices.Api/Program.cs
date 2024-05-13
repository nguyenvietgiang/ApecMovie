using FluentValidation;
using Minio;
using MovieServices.Application.BussinessServices;
using MovieServices.Application.Mapping;
using MovieServices.Application.ModelsDTO;
using MovieServices.Application.Validator;
using MovieServices.Domain.Interfaces;
using MovieServices.Infrastructure.Context;
using MovieServices.Infrastructure.Repository;
using ApecMovieCore.Middlewares;
using SwaggerDoc;
using CoreHeathCheck;

using ApecCoreIdentity;
using IoCmanage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Thêm customservices từ IocManager.
builder.Services.AddCustomServices();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.ConfigureSwaggerAndAuth("APEC Movie Services", "Movie Services for APEC Backend Microservices");

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

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

var dbconnection = "Server=localhost;Port=5432;Database=ApecMovieData;User Id=postgres;Password=vip1111;";

builder.Services.AddCustomHealthChecks(dbconnection);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("_myAllowSpecificOrigins");

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

app.MapCustomHealthChecks();

app.Run();


