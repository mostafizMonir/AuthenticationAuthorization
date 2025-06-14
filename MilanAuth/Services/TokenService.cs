using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace MilanAuth.Services;

public class TokenService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly ILogger<TokenService> _logger;

    public TokenService(HttpClient httpClient, IConfiguration config, ILogger<TokenService> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _logger = logger;
    }

    public async Task<string?> GetAccessTokenAsync(string username, string password)
    {
        _logger.LogInformation("Attempting to get access token for user: {Username}", username);
        
        var tokenUrl = _config["Keycloak:TokenUrl"];
        var clientId = _config["Keycloak:ClientId"];
        var clientSecret = _config["Keycloak:ClientSecret"];

        var parameters = new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"] = clientId,
            ["username"] = username,
            ["password"] = password
        };

        _logger.LogInformation("client id : {clientId}", clientId);
        _logger.LogInformation($"parameters : {parameters.ToString()}");

        // if (!string.IsNullOrEmpty(clientSecret))
        //     parameters["client_secret"] = clientSecret;

        try
        {
            var response = await _httpClient.PostAsync(tokenUrl, new FormUrlEncodedContent(parameters));
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get access token for user: {Username}. Status code: {StatusCode}", 
                    username, response.StatusCode);
                return null;
            }

            var body = await response.Content.ReadAsStringAsync();
            var tokenObj = JsonSerializer.Deserialize<JsonElement>(body);
            var token = tokenObj.GetProperty("access_token").GetString();
            
            _logger.LogInformation("Successfully obtained access token for user: {Username}", username);
            return token;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting access token for user: {Username}", username);
            throw;
        }
    }
}
