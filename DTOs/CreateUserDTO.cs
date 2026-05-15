using AcaHelpAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace AcaHelpAPI.DTOs
{
    public record CreateUserResponseDTO
    {
        public required int  Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
    public class CreateUserDTO
    {
        [Required]
        [StringLength(30, MinimumLength = 2)]
        public string Name { get; set; }

        [StringLength(30)]
        public string? LastName { get; set; }  // nullable = opcional

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

    }
}
