using Microsoft.AspNetCore.Identity;

namespace SecondRoundProject.Models
{
    public class ApplicationUser
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public required string Role { get; set; }
    }
}
