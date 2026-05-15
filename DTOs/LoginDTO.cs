using System.ComponentModel.DataAnnotations;

namespace AcaHelpAPI.DTOs
{
    public record LoginResponseDTO
    {
        public required string token { get; set; }
        public required DateTime date { get; set; } 

        public required DateTime expiresIn {  get; set; }
    }
    public class LoginDTO
    {

        [Required]
        [MinLength(5)]
        public string Email { get; set; } = string.Empty;
        [Required]
        [MinLength(1)]
        public string Password { get; set; } = string.Empty;
    }
}
