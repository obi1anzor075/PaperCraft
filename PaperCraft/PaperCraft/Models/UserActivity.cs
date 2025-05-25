namespace PaperCraft.Models
{
    namespace PaperCraft.Models
    {
        public class UserActivity
        {
            public int Id { get; set; }
            public string UserId { get; set; }
            public AppUser User { get; set; }
            public string Action { get; set; }
            public string Description { get; set; }
            public string? IPAddress { get; set; }
            public string? UserAgent { get; set; }
            public DateTime CreatedAt { get; set; }
            public ActivityType Type { get; set; }
        }

        public enum ActivityType
        {
            Login,
            ProfileUpdate,
            OrderPlaced,
            OrderDelivered,
            Registration,
            PasswordChange,
            Other
        }
    }
}
