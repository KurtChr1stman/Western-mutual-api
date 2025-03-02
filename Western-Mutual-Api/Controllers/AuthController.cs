using Microsoft.AspNetCore.Mvc;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly JwtService _jwtService;

    public AuthController(JwtService jwtService)
    {
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // Dummy user validation (replace with database validation)
        if (request.Username == "testuser" && request.Password == "password123")
        {
            var token = _jwtService.GenerateToken(request.Username);
            return Ok(new { Token = token });
        }

        return Unauthorized(new { Message = "Invalid credentials" });
    }
}

public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}
