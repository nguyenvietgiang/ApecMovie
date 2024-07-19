using LogServices.API.Models;
using LogServices.API.Services;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using ApecMovieCore.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDBSettings>(
    builder.Configuration.GetSection("MongoDBSettings"));

var mongoSettings = builder.Configuration.GetSection("MongoDBSettings").Get<MongoDBSettings>();

// Cấu hình MongoDB client
var mongoClientSettings = MongoClientSettings.FromConnectionString(mongoSettings.ConnectionString);
mongoClientSettings.GuidRepresentation = GuidRepresentation.Standard;
mongoClientSettings.DirectConnection = true;
mongoClientSettings.ServerApi = new ServerApi(ServerApiVersion.V1);

// Cấu hình convention pack để xử lý tên collection không phân biệt chữ hoa/thường
var conventionPack = new ConventionPack { new CamelCaseElementNameConvention() };
ConventionRegistry.Register("camelCase", conventionPack, t => true);

var mongoClient = new MongoClient(mongoClientSettings);
var database = mongoClient.GetDatabase(mongoSettings.DatabaseName);

// Add services to the container.
builder.Services.AddSingleton<IMongoClient>(mongoClient);
builder.Services.AddSingleton(database);
builder.Services.AddScoped<ILogService, LogService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
// Configure the HTTP request pipeline.

app.UseErrorHandlingMiddleware();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();