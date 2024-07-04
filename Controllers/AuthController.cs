using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SecondRoundProject.DTOs;
using SecondRoundProject.Services;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IUserService userService, ILogger<AuthController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO model)
    {
        _logger.LogInformation("Register endpoint called");

        var (success, message) = await _userService.RegisterAsync(model);
        if (!success)
        {
            _logger.LogError("Registration failed: {message}", message);
            return BadRequest(new { message });
        }

        _logger.LogInformation("User registered successfully: {Username}", model.Username);
        return Ok(new { message });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO model)
    {
        _logger.LogInformation("Login endpoint called");

        var (success, message, token) = await _userService.LoginAsync(model);
        if (!success)
        {
            _logger.LogError("Login failed: {message}", message);
            return Unauthorized(new { message });
        }

        _logger.LogInformation("User logged in successfully: {Username}", model.Username);
        return Ok(new { message, token });
    }
}
