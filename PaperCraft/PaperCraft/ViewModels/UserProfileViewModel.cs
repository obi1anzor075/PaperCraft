using System.ComponentModel.DataAnnotations;
using PaperCraft.Models;
using PaperCraft.Models.PaperCraft.Models;

namespace PaperCraft.ViewModels
{
    public class UserProfileViewModel
    {
        [Required(ErrorMessage = "Имя обязательно")]
        [StringLength(50, ErrorMessage = "Имя не должно превышать 50 символов")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Фамилия обязательна")]
        [StringLength(50, ErrorMessage = "Фамилия не должна превышать 50 символов")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Некорректный формат телефона")]
        public string PhoneNumber { get; set; }

        [StringLength(500, ErrorMessage = "Описание не должно превышать 500 символов")]
        public string Bio { get; set; }
    }

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
        public ActivityType Type { get; set; }
        public string IPAddress { get; set; }
        public string TimeAgo { get; set; }
        public string TypeText => GetTypeText();
        public string TypeIcon => GetTypeIcon();
        public string TypeColor => GetTypeColor();

        private string GetTypeText()
        {
            return Type switch
            {
                ActivityType.Login => "Вход",
                ActivityType.ProfileUpdate => "Профиль",
                ActivityType.OrderPlaced => "Заказ",
                ActivityType.OrderDelivered => "Доставка",
                ActivityType.Registration => "Регистрация",
                ActivityType.PasswordChange => "Безопасность",
                _ => "Прочее"
            };
        }

        private string GetTypeIcon()
        {
            return Type switch
            {
                ActivityType.Login => "fas fa-sign-in-alt",
                ActivityType.ProfileUpdate => "fas fa-user-edit",
                ActivityType.OrderPlaced => "fas fa-shopping-cart",
                ActivityType.OrderDelivered => "fas fa-truck",
                ActivityType.Registration => "fas fa-user-plus",
                ActivityType.PasswordChange => "fas fa-key",
                _ => "fas fa-info-circle"
            };
        }

        private string GetTypeColor()
        {
            return Type switch
            {
                ActivityType.Login => "text-success",
                ActivityType.ProfileUpdate => "text-info",
                ActivityType.OrderPlaced => "text-primary",
                ActivityType.OrderDelivered => "text-success",
                ActivityType.Registration => "text-primary",
                ActivityType.PasswordChange => "text-warning",
                _ => "text-muted"
            };
        }
    }

    public class OrderSummaryViewModel
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public string StatusText { get; set; }
        public int ItemCount { get; set; }
        public string TrackingNumber { get; set; }
        public string StatusClass => GetStatusClass();
        public string StatusIcon => GetStatusIcon();

        private string GetStatusClass()
        {
            return Status switch
            {
                OrderStatus.Pending => "badge bg-warning",
                OrderStatus.Processing => "badge bg-info",
                OrderStatus.Shipped => "badge bg-primary",
                OrderStatus.Delivered => "badge bg-success",
                OrderStatus.Cancelled => "badge bg-danger",
                _ => "badge bg-secondary"
            };
        }

        private string GetStatusIcon()
        {
            return Status switch
            {
                OrderStatus.Pending => "fas fa-clock",
                OrderStatus.Processing => "fas fa-cog fa-spin",
                OrderStatus.Shipped => "fas fa-shipping-fast",
                OrderStatus.Delivered => "fas fa-check-circle",
                OrderStatus.Cancelled => "fas fa-times-circle",
                _ => "fas fa-question-circle"
            };
        }
    }

    public class OrderDetailsViewModel
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public string StatusText { get; set; }
        public string TrackingNumber { get; set; }
        public List<OrderItemViewModel> Items { get; set; }
        public string DeliveryAddress { get; set; }
        public DateTime? EstimatedDelivery { get; set; }
    }

    public class OrderItemViewModel
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total => Quantity * Price;
        public string ProductImageUrl { get; set; }
    }

    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public int ProductCount { get; set; }
    }
}