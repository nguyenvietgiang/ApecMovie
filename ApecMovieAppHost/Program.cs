var builder = DistributedApplication.CreateBuilder(args);

var movieApi = builder.AddProject<Projects.MovieServices_Api>("MovieServices-Api");
var userApi = builder.AddProject<Projects.UserServices_Api>("UserServices-Api");
var ticketApi = builder.AddProject<Projects.TicketServices_Api>("TicketServices-Api");
var emailApi = builder.AddProject<Projects.EmailServices_Api>("EmailServices-Api");
var blogApi = builder.AddProject<Projects.BlogServices>("BlogServices-Api");

builder.Build().Run();
