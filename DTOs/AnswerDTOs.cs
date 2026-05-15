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
}
