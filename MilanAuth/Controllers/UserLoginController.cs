using Microsoft.AspNetCore.Mvc;
using MilanAuth.Services;

[ApiController]
[Route("api/[controller]")]
public class UserLoginController : ControllerBase
{
    private readonly TokenService _tokenService;

    public UserLoginController(TokenService tokenService  )
    {
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var token = await _tokenService.GetAccessTokenAsync(request.Username, request.Password);
        if (token == null)
            return Unauthorized("Invalid credentials");

        return Ok(new { token });
    }
}

public class LoginRequest
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}
