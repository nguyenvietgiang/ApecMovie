using ApecCoreIdentity;
using FluentValidation;
using TicketServices.Application.BussinessServices;
using TicketServices.Application.Mapping;
using TicketServices.Application.ModelsDTO;
using TicketServices.Application.Validator;
using TicketServices.Domain.Interface;
using TicketServices.Infrastructure.Context;
using TicketServices.Infrastructure.Repository;
using SwaggerDoc;
using IoCmanage;

var builder = WebApplication.CreateBuilder(args);

// Thêm customservices từ IocManager.
builder.Services.AddCustomServices();

builder.Services.AddAutoMapper(typeof(MappingTicketProfile));

builder.Services.AddTransient<IValidator<TicketDTO>, TicketDTOValidator>();

builder.Services.AddDbContext<TicketDbContext>();

// Repository
builder.Services.AddScoped<ITicketRepository, TicketRepository>();

//Services
builder.Services.AddScoped<ITicketService, TicketServiceImp>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwaggerAndAuth("APEC Ticket Services", "Ticket Services for APEC Backend Microservices");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("_myAllowSpecificOrigins");


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
