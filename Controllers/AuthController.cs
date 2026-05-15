using AcaHelpAPI.Data;
using AcaHelpAPI.DTOs;
using AcaHelpAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
namespace AcaHelpAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly MiDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(MiDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

       
        [HttpPost("login")]
        public async Task<IActionResult> GetUser(LoginDTO loginDto)
        {

            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
                if (user is null)
                {
                    return Unauthorized(ApiResponse<object>.ErrorResponse("Credenciales inválidas", "INVALID_CREDENTIALS"));
                }

                bool isValidPassword = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.HashedPassword);
                if (!isValidPassword)
                {
                    return Unauthorized(ApiResponse<object>.ErrorResponse("Credenciales inválidas", "INVALID_CREDENTIALS"));
                }
                var expirationDate = DateTime.UtcNow.AddMinutes(60);
                var token = GenerateToken(user, expirationDate);
                var data = new LoginResponseDTO { date = DateTime.UtcNow, token = token, expiresIn= expirationDate };
                var payloadResponse = ApiResponse<LoginResponseDTO>.SuccessResponse(data, "USER_LOGGED_IN" , "Usuario logueado exitosamente");
                return Ok(payloadResponse);

            }
            catch (Exception ex) {
                return StatusCode(500,ApiResponse<object>.ErrorResponse("Error del sistema", "INTERNAL_SERVER_ERROR"));
            }
            

        }

        private string GenerateToken(User user, DateTime expirationDate)
        {
            var jwtConfig = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtConfig["Issuer"],
                audience: jwtConfig["Audience"],
                claims: claims,
                expires: expirationDate,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}
