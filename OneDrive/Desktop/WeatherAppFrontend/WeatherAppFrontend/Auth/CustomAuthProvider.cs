using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt; // Ensure this is recognized after package installation

namespace WeatherAppFrontend.Auth
{
    public class CustomAuthProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;

        public CustomAuthProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            var email = await _localStorage.GetItemAsync<string>("email");

            ClaimsIdentity identity = new ClaimsIdentity();

            Console.WriteLine($"Checking authentication: Token={token}, Stored Email={email}");
            if (!string.IsNullOrWhiteSpace(token))
            {
                var tokenEmail = GetEmailFromToken(token);
                email = tokenEmail ?? email;
                Console.WriteLine($"Token email: {tokenEmail}, Final email: {email}");

                if (!string.IsNullOrWhiteSpace(email))
                {
                    identity = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, email),
                        new Claim(JwtRegisteredClaimNames.Email, email)
                    }, "apiauth"); // "apiauth" is the authentication type (string)
                    Console.WriteLine("Authentication state set with email: " + email);
                }
                else
                {
                    Console.WriteLine("No valid email found for authentication.");
                }
            }
            else
            {
                Console.WriteLine("No token found in localStorage.");
            }

            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }

        public void NotifyAuthChanged()
        {
            Console.WriteLine("Notifying authentication state changed.");
            var task = GetAuthenticationStateAsync();
            NotifyAuthenticationStateChanged(task);
        }

        private string? GetEmailFromToken(string jwt)
        {
            try
            {
                var parts = jwt.Split('.');
                if (parts.Length != 3)
                {
                    Console.WriteLine("Invalid JWT format.");
                    return null;
                }

                var payload = parts[1];
                payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
                var jsonBytes = Convert.FromBase64String(payload);
                var json = Encoding.UTF8.GetString(jsonBytes);
                Console.WriteLine($"Token payload: {json}");

                var payloadData = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                if (payloadData?.TryGetValue(JwtRegisteredClaimNames.Email, out var emailObj) == true)
                {
                    var email = emailObj?.ToString();
                    Console.WriteLine($"Extracted email: {email}");
                    return email;
                }
                Console.WriteLine("No 'email' claim found.");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token decoding error: {ex.Message}");
                return null;
            }
        }
    }
}