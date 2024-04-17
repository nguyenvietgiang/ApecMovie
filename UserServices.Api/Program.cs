using ApecCoreIdentity;
using ApecMovieCore.Interface;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using UserServices.Application.BussinessServices;
using UserServices.Application.Mapping;
using UserServices.Application.ModelsDTO;
using UserServices.Application.Validator;
using UserServices.Domain.Interfaces;
using UserServices.Domain.Models;
using UserServices.Infrastructure.Context;
using UserServices.Infrastructure.Repository;

var builder = WebApplication.CreateBuilder(args);

// thêm để add validator
builder.Services.AddControllers()
        .AddFluentValidation(fv => fv.ImplicitlyValidateChildProperties = true);

// Add services to the container.

builder.Services.AddControllers();
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
        Title = "APEC Authentication Services",
        Description = "Authentication Services for APEC Backend Microservices"
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
