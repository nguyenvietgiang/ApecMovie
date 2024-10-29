var builder = DistributedApplication.CreateBuilder(args);
// api
// đối với Movie API nên chạy riêng vì đang cần chạy nhiều port để load balancer
//var movieApi = builder.AddProject<Projects.MovieServices_Api>("MovieServices-Api");

var userApi = builder.AddProject<Projects.UserServices_Api>("UserServices-Api");
var ticketApi = builder.AddProject<Projects.TicketServices_Api>("TicketServices-Api");
var emailApi = builder.AddProject<Projects.EmailServices_Api>("EmailServices-Api");
var blogApi = builder.AddProject<Projects.BlogServices>("BlogServices-Api");
var logApi = builder.AddProject<Projects.LogServices_API>("LogServices-Api");
var aggregatorApi = builder.AddProject<Projects.AggregatorService_API>("AggregatorService-Api");


// gateway
var gateway = builder.AddProject<Projects.ApecMovieGateWay>("Gateway");

// fe
builder.AddProject<Projects.ApecMoviePortal>("ApecMoviePortal");
builder.AddProject<Projects.ApecMovieManager>("ApecMovieManager");

builder.Build().Run();
