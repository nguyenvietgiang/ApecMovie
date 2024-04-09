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


var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

// thêm để add validator
builder.Services.AddControllers()
        .AddFluentValidation(fv => fv.ImplicitlyValidateChildProperties = true);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


