using ApecMoviePortal.Client;
using ApecMoviePortal.Middleware;
using ApecMoviePortal.Services.AuthServices;
using ApecMoviePortal.Services.MovieServices;
using ApecMoviePortal.Services.TicketServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddHttpClient<IMovieService, MovieService>();
builder.Services.AddHttpClient<IAuthService, AuthService>();

// paypal client configuration
builder.Services.AddSingleton(x =>
    new PaypalClient(
        builder.Configuration["PayPalOptions:ClientId"],
        builder.Configuration["PayPalOptions:ClientSecret"],
        builder.Configuration["PaypalOptions:Mode"]
    )
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//app.UseTokenValidation();
app.UseMiddleware<TokenMiddleware>();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
