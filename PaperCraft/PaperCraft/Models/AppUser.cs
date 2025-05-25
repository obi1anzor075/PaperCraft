using Microsoft.AspNetCore.Identity;

namespace PaperCraft.Models
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        public string? Bio { get; set; }
    }
}
