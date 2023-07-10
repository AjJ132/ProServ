global using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ProServ.Client;
using Radzen;
using ProServ.Client.Controllers;
using Blazored.LocalStorage;
using System.Net.Http.Headers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

// Determine base API URL based on environment
var baseApiUrl = builder.HostEnvironment.IsDevelopment()
    ? "https://localhost:5000"   // Replace with your actual development server URL
    : "https://proserv.azurewebsites.net";  // Replace with your actual production server URL

builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<DialogService>();

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(baseApiUrl)
});

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();
