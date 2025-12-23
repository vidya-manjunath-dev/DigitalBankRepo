using DigitalBankLite.API.DTOs;
using DigitalBankLite.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DigitalBankLite.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [Authorize]
        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            // Try NameIdentifier first, then sub
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                        ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userIdStr)) 
            {
                return Unauthorized(new { message = "User ID claim not found in token." });
            }

            if (!int.TryParse(userIdStr, out int userId))
            {
                return BadRequest(new { message = "Invalid User ID format in token." });
            }

            var profile = _authService.GetProfile(userId);
            
            if (profile == null) return NotFound(new { message = "Customer profile not found." });
            return Ok(profile);
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterDto dto)
        {
            var result = _authService.Register(dto);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }
            return Ok(new { message = result.Message });
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            var result = _authService.Login(dto);                    // Login Button 
            if (!result.Success)
            {
                // In a real app, distinguish 401 vs 404 vs 400
                return StatusCode(401, new { message = result.Message });
            }
            return Ok(result.Response);
        }

        [HttpGet("debug")]
        public IActionResult DebugUsers()
        {
            var users = _authService.DebugUsers();
            return Ok(users);
        }

        [HttpGet("reset-admin-password")]
        public IActionResult ResetAdminPassword()
        {
            _authService.ResetAdminPassword();
            return Ok(new { message = "Password for myadmin@bank.com reset to 'Admin@123'" });
        }
    }
}
