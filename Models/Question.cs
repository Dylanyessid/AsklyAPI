namespace AcaHelpAPI.Models
{
    public class Question: BaseModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
                
        public int TagId { get; set; }
        public string Title { get; set; }
        public string? Body { get; set; } 
        public bool IsSolved { get; set; }

    }
}
