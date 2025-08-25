using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation; // Added for AddRazorRuntimeCompilation
using MySecurityBank.Models;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

// EF Core using Azure SQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });

builder.Services.AddAntiforgery(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.SuppressXFrameOptionsHeader = true; // We'll set framing policy globally
});

var app = builder.Build();

// Security headers middleware
app.Use(async (context, next) =>
{
    // Enforce HTTPS and HSTS
    // HSTS middleware below will add HSTS on production. Here we ensure redirect is in place.
    // Clickjacking protection
    context.Response.Headers["X-Frame-Options"] = "DENY";
    // XSS protection and MIME sniffing
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["Referrer-Policy"] = "no-referrer";
    // Basic CSP: allow only self for scripts/styles; allow inline hashes for antiforgery token script emitted by TagHelpers
    var csp = "default-src 'self'; script-src 'self' https://code.jquery.com https://cdnjs.cloudflare.com; style-src 'self' https://cdn.jsdelivr.net https://cdnjs.cloudflare.com; img-src 'self' data:; frame-ancestors 'none'; base-uri 'self'; form-action 'self'";
    context.Response.Headers["Content-Security-Policy"] = csp;

    await next();
});

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();