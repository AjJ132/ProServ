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

string serverApiBaseUrl = builder.Configuration["ServerApi:BaseUrl"];


builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<DialogService>();


builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(serverApiBaseUrl)

});

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();


