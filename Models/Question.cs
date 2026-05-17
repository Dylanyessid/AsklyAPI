namespace AcaHelpAPI.Models
{
    public class Question
    {
        public int Id { get; set; }
        public int UserId { get; set; }
                
        public int TagId { get; set; }
        public string Title { get; set; }
        public string? Body { get; set; }  // nullable porque en SQL no tiene NOT NULL
        public string Subject { get; set; }

        public bool IsSolved { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
