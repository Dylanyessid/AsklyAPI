using System.ComponentModel.DataAnnotations;

namespace AcaHelpAPI.DTOs
{
    public class CreateQuestionDTO
    {
        [Required]
        [MinLength(10)]
        public string Title { get; set; }


        [Required]
        public string Body { get; set; }

        [Required]
        public string Subject { get; set; }

    }
}
