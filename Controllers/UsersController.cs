using AcaHelpAPI.Data;
using AcaHelpAPI.DTOs;
using AcaHelpAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;

namespace AcaHelpAPI.Controllers
{

   

    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly MiDbContext _context;

        public UsersController(MiDbContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Select(user => new UserResponseDTO
                {
                    Id = user.Id,
                    Name = user.Name,
                    LastName = user.LastName,
                    Email = user.Email,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                })
                .ToListAsync();

            return Ok(ApiResponse<List<UserResponseDTO>>.SuccessResponse(users, "USERS_GIVEN", "Users retrieved successfully"));
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.Users
                .Where(user => user.Id == id)
                .Select(user => new UserResponseDTO
                {
                    Id = user.Id,
                    Name = user.Name,
                    LastName = user.LastName,
                    Email = user.Email,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Usuario no encontrado", "USER_NOT_FOUND"));
            }

            return Ok(ApiResponse<UserResponseDTO>.SuccessResponse(user, "USER_GIVEN", "User retrieved successfully"));
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostUser(CreateUserDTO dto)
        {


            var isExistingUser = await _context.Users.Where(u => u.Email == dto.Email).AnyAsync();


            if (isExistingUser) return Conflict(ApiResponse<object>.ErrorResponse("Usuario con correo existente", "USER_EXISTING"));
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

           

            var user = new User {
                Name    = dto.Name,
                LastName = dto.LastName,
                Email = dto.Email,
                HashedPassword = hashedPassword,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var responseData = new CreateUserResponseDTO
            {
               Id = user.Id,
               Email = user.Email,
               Name = user.Name,
               LastName = user.LastName,
               UpdatedAt = user.UpdatedAt,
               CreatedAt = user.CreatedAt,
            };

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, ApiResponse<CreateUserResponseDTO>.SuccessResponse(responseData, "USER_CREATED" ,"Usuario creado exitosamente"));
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
