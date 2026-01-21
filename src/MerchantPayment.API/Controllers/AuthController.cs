using Microsoft.AspNetCore.Mvc;
using MerchantPayment.Application.DTOs;
using MerchantPayment.Application.Interfaces;

namespace MerchantPayment.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> Register([FromBody] RegisterUserRequest request)
    {
        var user = await _authService.RegisterAsync(request);
        return CreatedAtAction(
            nameof(Register),
            ApiResponse<UserResponse>.SuccessResponse(user, "User registered successfully"));
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        return Ok(ApiResponse<LoginResponse>.SuccessResponse(response, "Login successful"));
    }
}
