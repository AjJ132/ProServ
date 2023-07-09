using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using ProServ.Server.Controllers;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication.Cookies;
using ProServ.Server.Contexts;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddBlazoredLocalStorage();

//builder.Services.AddDbContext<ProServDbContext>(options =>
//    options.UseSqlServer(
//        configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContextFactory<ProServDbContext>(options =>
    options.UseSqlServer(
        configuration.GetConnectionString("DefaultConnection"),
		sqlServerOptionsAction: sqlOptions =>
		{
			sqlOptions.EnableRetryOnFailure(
				maxRetryCount: 5,
				maxRetryDelay: TimeSpan.FromSeconds(30),
				errorNumbersToAdd: null);
		}));

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Lockout.AllowedForNewUsers = false;
    options.Password.RequireNonAlphanumeric = false;

})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ProServDbContext>();




var app = builder.Build();

//Create roles
//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    try
//    {
//        await CreateRoles(services);
//    }
//    catch (Exception ex)
//    {
//        var logger = services.GetRequiredService<ILogger<Program>>();
//        logger.LogError(ex, "An error occurred while creating roles.");
//    }
//}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors(policy =>
    policy.WithOrigins("https://proservclient.azurewebsites.net")  // Client URL
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");





app.Run();



//Add roles here
static async Task CreateRoles(IServiceProvider serviceProvider)
{
    var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roleNames = { "Admin", "Member", "Guest", "Coach", "Assistant Coach", "Sudo" };
    foreach (var roleName in roleNames)
    {
        var roleExist = await RoleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            await RoleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}
