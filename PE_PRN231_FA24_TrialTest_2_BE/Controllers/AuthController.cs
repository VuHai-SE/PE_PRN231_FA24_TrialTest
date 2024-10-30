using Domain.Interfaces.Services;
using Dto.Requests;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PE_PRN231_FA24_TrialTest_2_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ViroCureFal2024dbContext _context;
        private readonly ITokenService _tokenService;

        public AuthController(ViroCureFal2024dbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.ViroCureUsers
                .FirstOrDefaultAsync(u => u.Email == request.email && u.Password == request.password);

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
                    role = user.Role == 1 ? "admin" : user.Role == 2 ? "benh nhan" : "bac si" // Mapping role to descriptive string
                }
            });
        }
    }
}
