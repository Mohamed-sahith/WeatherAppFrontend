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

// Register the AuthHeaderHandler with dependencies
builder.Services.AddScoped<AuthHeaderHandler>();
builder.Services.AddScoped(sp => new AuthHeaderHandler(
    sp.GetRequiredService<ILocalStorageService>(),
    sp.GetRequiredService<AuthenticationStateProvider>()
));

// Named HttpClient for API communication
builder.Services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri("https://localhost:7257/"); // Ensure this matches your API URL
    client.DefaultRequestHeaders.Add("Accept", "application/json"); // Keep Accept header
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

// Add MudBlazor services
builder.Services.AddMudServices();

var host = builder.Build();

try
{
    Console.WriteLine("Starting WebAssembly host...");
    await host.RunAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"Application startup error: {ex.Message}\nStackTrace: {ex.StackTrace}");
}