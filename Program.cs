using cinema_cs.Data;
using cinema_cs.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Configure logging (optional, for extra control)
builder.Logging.ClearProviders(); // Optional: clear default providers if you want only specific ones
builder.Logging.AddConsole();     // Add console logger
builder.Logging.AddDebug();       // Add debug logger (useful in Visual Studio)
builder.Logging.SetMinimumLevel(LogLevel.Information); // Adjust as needed

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Login";
    //   options.AccessDeniedPath = "/AccessDenied"; // Optional
    //
    options.Events.OnRedirectToLogin = context =>
    {
        var returnUrl = context.Request.Path + context.Request.QueryString;
        context.Response.Redirect($"/Login?returnUrl={Uri.EscapeDataString(returnUrl)}&reason=login-required");
        return Task.CompletedTask;
    };
});

builder.Services.AddAuthorization();

builder.Services.AddRazorPages();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate(); // <-- Ensure migrations are applied at startup
    DbInitializer.Seed(context);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
