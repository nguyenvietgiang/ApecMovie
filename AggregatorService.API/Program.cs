var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddHttpClient("TicketService", c =>
{
    c.BaseAddress = new Uri("https://localhost:7246/"); 
});

builder.Services.AddHttpClient("UserService", c =>
{
    c.BaseAddress = new Uri("https://localhost:7089/"); 
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
