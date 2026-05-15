namespace AcaHelpAPI.Models
{
    
  
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? LastName { get; set; }  // nullable porque en SQL no tiene NOT NULL
        public string Email { get; set; }

        public string HashedPassword { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

}