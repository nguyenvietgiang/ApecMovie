using ApecCoreIdentity;
using ApecMovieCore.Interface;
using FluentValidation;
using FluentValidation.AspNetCore;
using RabbitMQ.Connection;
using RabbitMQ.Event;
using UserServices.Application.BussinessServices;
using UserServices.Application.Mapping;
using UserServices.Application.ModelsDTO;
using UserServices.Application.Validator;
using UserServices.Domain.Interfaces;
using UserServices.Domain.Models;
using UserServices.Infrastructure.Context;
using UserServices.Infrastructure.Repository;
using SwaggerDoc;

var builder = WebApplication.CreateBuilder(args);

// thêm để add validator
builder.Services.AddControllers()
        .AddFluentValidation(fv => fv.ImplicitlyValidateChildProperties = true);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.ConfigureSwaggerAndAuth("APEC Authentication Services", "Authentication Services for APEC Backend Microservices");


// message broker
builder.Services.AddSingleton<IRabbitmqConnection>(new RabbitmqConnection());
builder.Services.AddScoped<IMessageProducer, RabbitmqProducer>();

builder.Services.AddDbContext<UserDbContext>();

builder.Services.AddTransient<IValidator<UserDTO>, UserDTOValidator>();

builder.Services.AddScoped<IRepository<User>, UserRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddAutoMapper(typeof(MappingUserProfile));


builder.Services.AddScoped<IUserService, UserServiceImplementation>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.Use(async (context, next) =>
{
    var middleware = new JwtMiddleware();
    await middleware.Invoke(context, next);
});
app.UseAuthorization();

app.MapControllers();

app.Run();
