using ApecCoreIdentity;
using ApecMovieCore.Interface;
using FluentValidation;
using UserServices.Application.BussinessServices;
using UserServices.Application.Mapping;
using UserServices.Application.ModelsDTO;
using UserServices.Application.Validator;
using UserServices.Domain.Interfaces;
using UserServices.Domain.Models;
using UserServices.Infrastructure.Context;
using UserServices.Infrastructure.Repository;
using SwaggerDoc;
using IoCmanage;
using gRPC_CoreServer.Services;
using Microsoft.Extensions.Options;
using GrpcEmailService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCustomServices();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.ConfigureSwaggerAndAuth("APEC Authentication Services", "Authentication Services for APEC Backend Microservices");


builder.Services.AddDbContext<UserDbContext>();

builder.Services.AddTransient<IValidator<UserDTO>, UserDTOValidator>();

builder.Services.AddScoped<IRepository<User>, UserRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

builder.Services.AddAutoMapper(typeof(MappingUserProfile));


builder.Services.AddScoped<IUserService, UserServiceImplementation>();

builder.Services.AddGrpc(o =>
{
    o.EnableDetailedErrors = true;
});

builder.Services.AddGrpcClient<EmailSender.EmailSenderClient>(options =>
{
    options.Address = new Uri("https://localhost:7277"); // Thay đổi địa chỉ phù hợp với Services 2
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("_myAllowSpecificOrigins");

app.UseHttpsRedirection();
app.UseAuthentication();
app.Use(async (context, next) =>
{
    var middleware = new JwtMiddleware();
    await middleware.Invoke(context, next);
});
app.UseAuthorization();

app.MapControllers();

app.MapGrpcService<EmailSenderService>();

app.Run();
