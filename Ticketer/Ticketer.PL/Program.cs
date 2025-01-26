using Microsoft.AspNetCore.Authentication.Cookies;
using Ticketer.PL.Components;
using Ticketer.DAL.AuthenticationService;
using Ticketer.DAL.Data;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("TicketerDB");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("The ConnectionString property has not been initialized.");
}

// Register the database context with the connection string
builder.Services.AddScoped<TicketManagementDatabaseContext>();
builder.Services.AddScoped<Ticketer.DAL.ConcertService.ConcertService>();
builder.Services.AddScoped<Ticketer.DAL.UserService.UserService>();
builder.Services.AddScoped<Ticketer.DAL.TicketService.TicketService>();
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpContextAccessor();



builder.Services.AddScoped<AuthenticationService>();

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//})
//    .AddCookie(options =>
//    {
//        options.LoginPath = "/Account/Login";
//        options.LogoutPath = "/Account/Logout";
//        options.AccessDeniedPath = "/Account/AccessDenied";
//        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
//        options.SlidingExpiration = true;
//    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

//app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
