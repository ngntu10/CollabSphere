using System.Text.Json.Serialization;

namespace CollabSphere.Modules.User.Models;

public class LoginUserModel
{
    public string Username { get; set; }

    public string Password { get; set; }

    public bool RememberMe { get; set; }
}


public class LoginResponseModel
{
    [JsonPropertyName("token")]
    public string Token { get; set; }

    [JsonPropertyName("expiresAt")]
    public string ExpiresAt { get; set; }

    [JsonPropertyName("account")]
    public AccountResponse account { get; set; }
}

public class AccountResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Username { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    public AccountResponse() { }

    public AccountResponse(string id, string userName, string email)
    {
        Id = id;
        Username = userName;
        Email = email;
    }
}
