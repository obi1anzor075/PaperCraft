using PaperCraft.Models.PaperCraft.Models;
using PaperCraft.Models;

namespace PaperCraft.ViewModels
{
    public class ProfileDataViewModel
    {
        public UserProfileViewModel Profile { get; set; }
        public ProfileStatsViewModel Stats { get; set; }
        public List<UserActivityViewModel> RecentActivity { get; set; }
        public List<OrderSummaryViewModel> RecentOrders { get; set; }
    }

    public class ProfileStatsViewModel
    {
        public int TotalOrders { get; set; }
        public int YearsWithUs { get; set; }
        public decimal Rating { get; set; }
        public int BonusPoints { get; set; }
    }

    public class UserActivityViewModel
    {
        public string Action { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string TimeAgo { get; set; }
        public ActivityType Type { get; set; }
        public string IPAddress { get; set; }
    }

    public class OrderSummaryViewModel
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public string StatusText { get; set; }
        public int ItemCount { get; set; }
    }
}