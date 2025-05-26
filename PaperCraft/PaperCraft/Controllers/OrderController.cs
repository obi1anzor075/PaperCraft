using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaperCraft.Data;
using PaperCraft.Models;
using PaperCraft.ViewModels;

namespace PaperCraft.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public OrderController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// GET: /Order/PlaceOrder
        /// Возвращает пустую форму оформления заказа
        /// </summary>
        [HttpGet]
        public IActionResult PlaceOrder()
        {
            var vm = new PlaceOrderViewModel
            {
                ShippingAddress = "",
                Items = new List<OrderItemViewModel>()
            };
            // _PlaceOrderPartial.cshtml — partial с формой и textarea для адреса
            return PartialView("_PlaceOrderPartial", vm);
        }

        /// <summary>
        /// POST: /Order/MyPartial
        /// Рендерит карточки товаров из корзины
        /// </summary>
        [HttpPost]
        public IActionResult MyPartial([FromBody] PlaceOrderViewModel vm)
        {
            if (vm == null)
                return BadRequest("Неверные данные");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (vm.Items == null || !vm.Items.Any())
            {
                // _CartEmptyPartial.cshtml — показывает «Корзина пуста»
                return PartialView("_CartEmptyPartial");
            }

            // _CartPartial.cshtml — partial, который принимает PlaceOrderViewModel и выводит список товаров
            return PartialView("_CartPartial", vm);
        }

        /// <summary>
        /// POST: /Order/PlaceOrder
        /// Сохраняет заказ в базе и возвращает JSON { success, orderId }
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderViewModel vm)
        {
            if (vm == null)
                return BadRequest(new { success = false, error = "Неверные данные" });

            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });

            if (vm.Items == null || !vm.Items.Any())
                return BadRequest(new { success = false, error = "Корзина пуста" });

            if (vm.Items.Any(i => i.Quantity < 1))
                return BadRequest(new { success = false, error = "Количество должно быть ≥1" });

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var order = new Order
            {
                UserId = user.Id,
                OrderDate = DateTime.UtcNow,
                ShippingAddress = vm.ShippingAddress,
                Status = OrderStatus.Pending,
                TotalAmount = vm.Items.Sum(i => i.Price * i.Quantity)
            };

            foreach (var item in vm.Items)
            {
                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                });
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return Json(new { success = true, orderId = order.Id });
        }

        /// <summary>
        /// GET: /Order/MyOrders
        /// Возвращает список ваших заказов
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var orders = await _context.Orders
                .Where(o => o.UserId == user.Id)
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderSummaryViewModel
                {
                    Id = o.Id,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    StatusText = o.Status.ToString(),
                    ItemCount = o.OrderItems.Count
                })
                .ToListAsync();

            // _MyOrdersPartial.cshtml — partial с моделью IEnumerable<OrderSummaryViewModel>
            return PartialView("_MyOrdersPartial", orders);
        }
    }
}
