namespace AcaHelpAPI.Models
{
    public class AnswerVote: BaseModel
    {
        public int Id { get; set; }
        public int AnswerId { get; set; }
        public int UserId { get; set; }
        public sbyte VoteType { get; set; }
        
    }
}
