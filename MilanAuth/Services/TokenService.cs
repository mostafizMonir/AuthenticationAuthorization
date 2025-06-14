using System.Text.Json;

namespace MilanAuth.Services;

public class TokenService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public TokenService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<string?> GetAccessTokenAsync(string username, string password)
    {
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

        if (!string.IsNullOrEmpty(clientSecret))
            parameters["client_secret"] = clientSecret;

        var response = await _httpClient.PostAsync(tokenUrl, new FormUrlEncodedContent(parameters));
        if (!response.IsSuccessStatusCode)
            return null;

        var body = await response.Content.ReadAsStringAsync();
        var tokenObj = JsonSerializer.Deserialize<JsonElement>(body);
        return tokenObj.GetProperty("access_token").GetString();
    }
}
