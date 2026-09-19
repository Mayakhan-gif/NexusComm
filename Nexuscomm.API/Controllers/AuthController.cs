using Microsoft.AspNetCore.Mvc;
using Nexuscomm.API.DTOs;
using Nexuscomm.API.Services.Interfaces;

namespace Nexuscomm.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            return Ok(ApiResponseDto<AuthResponseDto>.SuccessResponse(result, "Registration successful."));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            return Ok(ApiResponseDto<AuthResponseDto>.SuccessResponse(result, "Login successful."));
        }
    }
}