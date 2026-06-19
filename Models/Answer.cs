namespace AcaHelpAPI.Models
{
    public class Answer: BaseModel
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public int UserId { get; set; }
        public string Body { get; set; }
        public bool IsAccepted { get; set; }
        public int VoteCount { get; set; }
     
    }
}
