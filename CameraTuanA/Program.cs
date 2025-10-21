using CameraTuanA.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<DBContext>(options =>
{
    var configuration = builder.Configuration; 
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Host.UseSerilog((context, config) =>
{
    config
        .Filter.ByExcluding(logEvent =>
            logEvent.Properties.ContainsKey("SourceContext") &&
            (
                logEvent.Properties["SourceContext"].ToString().StartsWith("\"Microsoft") ||
                logEvent.Properties["SourceContext"].ToString().StartsWith("\"System")
            )
        )
        .MinimumLevel.Information()
        .WriteTo.Console()
        .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day);
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Denied";
    });
builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
