using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaperCraft.Models;
using PaperCraft.ViewModels;
using PaperCraft.Services;
using PaperCraft.Models.PaperCraft.Models;
using PaperCraft.Data;

public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userMgr;
    private readonly SignInManager<AppUser> _signInMgr;
    private readonly AppDbContext _context;
    private readonly IUserActivityService _activityService;

    public AccountController(
        UserManager<AppUser> userMgr,
        SignInManager<AppUser> signInMgr,
        AppDbContext context,
        IUserActivityService activityService)
    {
        _userMgr = userMgr;
        _signInMgr = signInMgr;
        _context = context;
        _activityService = activityService;
    }

    [HttpGet]
    public IActionResult Register() => PartialView("_RegisterPartial");

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel m)
    {
        if (!ModelState.IsValid) return PartialView("_RegisterPartial", m);

        var user = new AppUser
        {
            FirstName = m.FirstName,
            LastName = m.LastName,
            UserName = m.Email,
            Email = m.Email,
            PhoneNumber = m.PhoneNumber,
            RegistrationDate = DateTime.UtcNow
        };

        var result = await _userMgr.CreateAsync(user, m.Password);
        if (result.Succeeded)
        {
            await _signInMgr.SignInAsync(user, isPersistent: false);

            // Логируем регистрацию
            await _activityService.LogActivityAsync(user.Id, "Регистрация", "Добро пожаловать!",
                GetClientIP(), Request.Headers["User-Agent"], ActivityType.Registration);

            return Json(new { success = true });
        }

        foreach (var err in result.Errors)
            ModelState.AddModelError("", err.Description);
        return PartialView("_RegisterPartial", m);
    }

    [HttpGet]
    public IActionResult Login() => PartialView("_LoginPartial");

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel m)
    {
        if (!ModelState.IsValid) return PartialView("_LoginPartial", m);

        var result = await _signInMgr.PasswordSignInAsync(m.Email, m.Password, m.RememberMe, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            var user = await _userMgr.FindByEmailAsync(m.Email);
            if (user != null)
            {
                // Логируем вход
                await _activityService.LogActivityAsync(user.Id, "Вход в систему", $"IP: {GetClientIP()}",
                    GetClientIP(), Request.Headers["User-Agent"], ActivityType.Login);
            }
            return Json(new { success = true });
        }

        ModelState.AddModelError("", "Неверные учетные данные");
        return PartialView("_LoginPartial", m);
    }

    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var user = await _userMgr.GetUserAsync(User);
        if (user == null) return Unauthorized();

        // Получаем статистику пользователя
        var stats = await GetUserStatsAsync(user.Id);

        // Получаем последнюю активность
        var recentActivity = await GetRecentActivityAsync(user.Id);

        // Получаем последние заказы
        var recentOrders = await GetRecentOrdersAsync(user.Id);

        var profileData = new ProfileDataViewModel
        {
            Profile = new UserProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber ?? "",
                Bio = user.Bio ?? ""
            },
            Stats = stats,
            RecentActivity = recentActivity,
            RecentOrders = recentOrders
        };

        return Json(profileData);
    }

    [HttpPost]
    public async Task<IActionResult> Profile([FromBody] UserProfileViewModel m)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = await _userMgr.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var changes = new List<string>();

        if (user.FirstName != m.FirstName)
        {
            changes.Add("имя");
            user.FirstName = m.FirstName;
        }

        if (user.LastName != m.LastName)
        {
            changes.Add("фамилия");
            user.LastName = m.LastName;
        }

        if (user.Bio != m.Bio)
        {
            changes.Add("описание");
            user.Bio = m.Bio;
        }

        if (m.Email != user.Email)
        {
            var emailRes = await _userMgr.SetEmailAsync(user, m.Email);
            if (!emailRes.Succeeded)
            {
                foreach (var e in emailRes.Errors)
                    ModelState.AddModelError("", e.Description);
                return BadRequest(ModelState);
            }
            changes.Add("email");
        }

        if (user.PhoneNumber != m.PhoneNumber)
        {
            changes.Add("телефон");
            user.PhoneNumber = m.PhoneNumber;
        }

        var updateRes = await _userMgr.UpdateAsync(user);
        if (!updateRes.Succeeded)
        {
            foreach (var e in updateRes.Errors)
                ModelState.AddModelError("", e.Description);
            return BadRequest(ModelState);
        }

        // Логируем изменения профиля
        if (changes.Any())
        {
            var changesText = string.Join(", ", changes);
            await _activityService.LogActivityAsync(user.Id, "Обновление профиля",
                $"Изменены: {changesText}", GetClientIP(), Request.Headers["User-Agent"], ActivityType.ProfileUpdate);
        }

        return Ok(new { success = true });
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders(int page = 1, int pageSize = 10)
    {
        var user = await _userMgr.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var orders = await _context.Orders
            .Where(o => o.UserId == user.Id)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .OrderByDescending(o => o.OrderDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status.ToString(),
                StatusText = GetStatusText(o.Status),
                ItemCount = o.OrderItems.Count,
                Items = o.OrderItems.Select(oi => new
                {
                    ProductName = oi.Product.Name,
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList(),
                TrackingNumber = o.TrackingNumber
            })
            .ToListAsync();

        var totalCount = await _context.Orders.CountAsync(o => o.UserId == user.Id);

        return Json(new { orders, totalCount, totalPages = (int)Math.Ceiling((double)totalCount / pageSize) });
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        var user = await _userMgr.GetUserAsync(User);
        if (user != null)
        {
            await _activityService.LogActivityAsync(user.Id, "Выход из системы", "До свидания!",
                GetClientIP(), Request.Headers["User-Agent"], ActivityType.Other);
        }

        await _signInMgr.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    private async Task<ProfileStatsViewModel> GetUserStatsAsync(string userId)
    {
        var user = await _context.Users.FindAsync(userId);
        var totalOrders = await _context.Orders.CountAsync(o => o.UserId == userId);
        var yearsWithUs = user?.RegistrationDate != null
            ? DateTime.UtcNow.Year - user.RegistrationDate.Year
            : 0;

        // Простая логика для рейтинга и бонусов
        var completedOrders = await _context.Orders
            .CountAsync(o => o.UserId == userId && o.Status == OrderStatus.Delivered);
        var rating = completedOrders > 0 ? Math.Min(5.0m, 3.5m + (completedOrders * 0.1m)) : 4.0m;
        var bonusPoints = totalOrders * 100 + completedOrders * 200;

        return new ProfileStatsViewModel
        {
            TotalOrders = totalOrders,
            YearsWithUs = Math.Max(yearsWithUs, 0),
            Rating = Math.Round(rating, 1),
            BonusPoints = bonusPoints
        };
    }

    private async Task<List<UserActivityViewModel>> GetRecentActivityAsync(string userId)
    {
        var activities = await _context.UserActivities
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .Take(10)
            .Select(a => new UserActivityViewModel
            {
                Action = a.Action,
                Description = a.Description,
                CreatedAt = a.CreatedAt,
                Type = a.Type,
                IPAddress = a.IPAddress ?? ""
            })
            .ToListAsync();

        foreach (var activity in activities)
        {
            activity.TimeAgo = GetTimeAgo(activity.CreatedAt);
        }

        return activities;
    }

    private async Task<List<OrderSummaryViewModel>> GetRecentOrdersAsync(string userId)
    {
        return await _context.Orders
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .Take(5)
            .Select(o => new OrderSummaryViewModel
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                StatusText = GetStatusText(o.Status),
                ItemCount = o.OrderItems.Count
            })
            .ToListAsync();
    }

    private string GetClientIP()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }

    private string GetTimeAgo(DateTime dateTime)
    {
        var timeSpan = DateTime.UtcNow - dateTime;

        if (timeSpan.TotalMinutes < 1) return "только что";
        if (timeSpan.TotalMinutes < 60) return $"{(int)timeSpan.TotalMinutes} мин назад";
        if (timeSpan.TotalHours < 24) return $"{(int)timeSpan.TotalHours} ч назад";
        if (timeSpan.TotalDays < 7) return $"{(int)timeSpan.TotalDays} дн назад";
        if (timeSpan.TotalDays < 30) return $"{(int)(timeSpan.TotalDays / 7)} нед назад";
        if (timeSpan.TotalDays < 365) return $"{(int)(timeSpan.TotalDays / 30)} мес назад";

        return $"{(int)(timeSpan.TotalDays / 365)} г назад";
    }

    private string GetStatusText(OrderStatus status)
    {
        return status switch
        {
            OrderStatus.Pending => "Ожидает",
            OrderStatus.Processing => "Обрабатывается",
            OrderStatus.Shipped => "Отправлен",
            OrderStatus.Delivered => "Доставлен",
            OrderStatus.Cancelled => "Отменен",
            _ => "Неизвестно"
        };
    }
}