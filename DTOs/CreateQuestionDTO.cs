using System.ComponentModel.DataAnnotations;

namespace AcaHelpAPI.DTOs
{
    public class CreateQuestionDTO
    {
        [Required]
        [MinLength(10)]
        public string title { get; set; }


        [Required]
        public string body { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "TagId must be a positive integer.")]
        public int tagId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "UserId must be a positive integer.")]
        public int userId { get; set; }


    }

    public class CreateQuestionResponseDTO
    {
        public required int id { get; set; }
        public string title { get; set; } = string.Empty;
        public string body { get; set; } = string.Empty;
        public int tagId { get; set; }
        public int userId { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class QuestionResponseDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TagId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Body { get; set; }
        public bool IsSolved { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
