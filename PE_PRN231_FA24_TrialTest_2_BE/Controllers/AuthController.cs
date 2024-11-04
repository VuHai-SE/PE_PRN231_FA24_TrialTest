using Domain.Interfaces.Services;
using Dto.Requests;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PE_PRN231_FA24_TrialTest_2_BE.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IViroCureUserService _viroCureUserService;
        private readonly ITokenService _tokenService;

        public AuthController(IViroCureUserService viroCureUserService, ITokenService tokenService)
        {
            _viroCureUserService = viroCureUserService;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _viroCureUserService.Login(request.email, request.password);

            if (user == null)
            {
                return Unauthorized(new { error = "Invalid email or password" });
            }

            var token = _tokenService.GenerateJwtToken(user);
            return Ok(new
            {
                message = "Login successful",
                token = token,
                user = new
                {
                    id = user.UserId,
                    email = user.Email,
                    role = user.Role == 1 ? "admin" : user.Role == 2 ? "patient" : "doctor" // Mapping role to descriptive string
                }
            });
        }
    }
}
