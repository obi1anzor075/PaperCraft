using System.ComponentModel.DataAnnotations;

namespace PaperCraft.ViewModels
{
    public class PlaceOrderViewModel
    {
        [Required]
        public string ShippingAddress { get; set; }

        [Required]
        public List<OrderItemViewModel> Items { get; set; }
    }
}