using System.ComponentModel.DataAnnotations;

namespace AcaHelpAPI.DTOs
{
    public class CreateAnswerDTO
    {
        [Required]
        [MinLength(5)]
        public string Body { get; set; } = string.Empty;
    }

    public class UpdateAnswerDTO
    {
        [Required]
        [MinLength(5)]
        public string Body { get; set; } = string.Empty;
    }

    public class CreateAnswerVoteDTO : IValidatableObject
    {
        [Required]
        public sbyte VoteType { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (VoteType != 1 && VoteType != -1)
            {
                yield return new ValidationResult(
                    "VoteType debe ser 1 o -1.",
                    new[] { nameof(VoteType) });
            }
        }
    }

    public class AnswerListItemDTO
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public int UserId { get; set; }
        public string Body { get; set; } = string.Empty;
        public bool IsAccepted { get; set; }
        public int VoteCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class AnswerListPageDTO
    {
        public List<AnswerListItemDTO> Items { get; set; } = new();
        public string? NextCursor { get; set; }
        public bool HasMore { get; set; }
    }
}
