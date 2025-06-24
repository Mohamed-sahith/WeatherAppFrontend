using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WeatherAppFrontend;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using WeatherAppFrontend.Auth;
using WeatherAppFrontend.Services;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Register the AuthHeaderHandler first
builder.Services.AddScoped<AuthHeaderHandler>();

// Named HttpClient for API communication (your backend)
builder.Services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri("https://localhost:7257/"); // Ensure this matches your backend URL
    client.DefaultRequestHeaders.Add("Accept", "application/json"); // Optional: Ensure JSON responses
})
.AddHttpMessageHandler<AuthHeaderHandler>();

// Session storage for JWT & user data
builder.Services.AddBlazoredLocalStorage();

// Custom authentication setup
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthProvider>();
builder.Services.AddAuthorizationCore();

// Your services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<WeatherService>();

// Add MudBlazor services to register ISnackbar and other MudBlazor dependencies
builder.Services.AddMudServices();

await builder.Build().RunAsync();