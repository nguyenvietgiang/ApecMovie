var builder = DistributedApplication.CreateBuilder(args);
// api ->với MovieAPI chạy 3 port để load balancer, 7207 đc aspire tự động nhận nên chỉ cần cấu hình 7205, 7206
var movieApi = builder.AddProject<Projects.MovieServices_Api>("MovieServices-Api")
                      .WithHttpsEndpoint(port: 7205, name: "https1")
                      .WithHttpsEndpoint(port: 7206, name: "https2");


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
