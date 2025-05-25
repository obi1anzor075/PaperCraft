using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PaperCraft.Models;
using PaperCraft.Models.PaperCraft.Models;

namespace PaperCraft.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка связей
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserActivity>()
                .HasOne(ua => ua.User)
                .WithMany()
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Настройка точности для десятичных чисел
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            // Заполнение категорий
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Ручки", Description = "Шариковые, гелевые ручки, маркеры", Icon = "fas fa-pen" },
                new Category { Id = 2, Name = "Тетради", Description = "Тетради различных форматов", Icon = "fas fa-book" },
                new Category { Id = 3, Name = "Маркер", Description = "Маркеры для выделения и письма", Icon = "fas fa-highlighter" },
                new Category { Id = 4, Name = "Карандаши", Description = "Простые и цветные карандаши", Icon = "fas fa-pencil-alt" },
                new Category { Id = 5, Name = "Блокноты", Description = "Блокноты и записные книжки", Icon = "fas fa-sticky-note" },
                new Category { Id = 6, Name = "Офис", Description = "Офисные принадлежности", Icon = "fas fa-briefcase" },
                new Category { Id = 7, Name = "Папки", Description = "Папки и регистраторы", Icon = "fas fa-folder" },
                new Category { Id = 8, Name = "Творчество", Description = "Товары для творчества", Icon = "fas fa-palette" },
                new Category { Id = 9, Name = "Канцтовары", Description = "Различные канцелярские товары", Icon = "fas fa-paperclip" },
                new Category { Id = 10, Name = "Клей", Description = "Клеящие материалы", Icon = "fas fa-prescription-bottle" },
                new Category { Id = 11, Name = "Обложки", Description = "Обложки для тетрадей и книг", Icon = "fas fa-shield-alt" },
                new Category { Id = 12, Name = "Аксессуары", Description = "Школьные и офисные аксессуары", Icon = "fas fa-tools" }
            );

            // Заполнение продуктов с привязкой к категориям
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Гелевая ручка", Description = "Гладкое письмо, чёрные чернила", Price = 49.99m, CategoryId = 1, ImageUrl = "/images/pen1.jpg" },
                new Product { Id = 2, Name = "Тетрадь 96 л.", Description = "Клетка, обложка с рисунком", Price = 89.00m, CategoryId = 2, ImageUrl = "/images/notebook1.jpg" },
                new Product { Id = 3, Name = "Маркеры для выделения", Description = "Набор из 6 неоновых цветов", Price = 149.50m, CategoryId = 3, ImageUrl = "/images/markers.jpg" },
                new Product { Id = 4, Name = "Карандаш механический", Description = "С ластиком, 0.5 мм", Price = 69.00m, CategoryId = 4, ImageUrl = "/images/mech-pencil.jpg" },
                new Product { Id = 5, Name = "Блокнот A5", Description = "На спирали, 120 стр.", Price = 129.00m, CategoryId = 5, ImageUrl = "/images/notepad.jpg" },
                new Product { Id = 6, Name = "Скрепки", Description = "Металлические, 100 шт.", Price = 35.00m, CategoryId = 6, ImageUrl = "/images/paperclips.jpg" },
                new Product { Id = 7, Name = "Папка-регистратор", Description = "Формат A4, синяя", Price = 199.00m, CategoryId = 7, ImageUrl = "/images/folder.jpg" },
                new Product { Id = 8, Name = "Калькулятор", Description = "12-разрядный дисплей", Price = 499.00m, CategoryId = 6, ImageUrl = "/images/calculator.jpg" },
                new Product { Id = 9, Name = "Цветные карандаши", Description = "Набор 24 цвета", Price = 189.90m, CategoryId = 8, ImageUrl = "/images/color-pencils.jpg" },
                new Product { Id = 10, Name = "Ластик", Description = "Не оставляет следов", Price = 29.00m, CategoryId = 9, ImageUrl = "/images/eraser.jpg" },
                new Product { Id = 11, Name = "Клей-карандаш", Description = "15 г", Price = 49.00m, CategoryId = 10, ImageUrl = "/images/glue-stick.jpg" },
                new Product { Id = 12, Name = "Ножницы офисные", Description = "Удобные ручки, 21 см", Price = 119.00m, CategoryId = 6, ImageUrl = "/images/scissors.jpg" },
                new Product { Id = 13, Name = "Стикеры-закладки", Description = "Набор разноцветных", Price = 39.99m, CategoryId = 6, ImageUrl = "/images/sticky-tabs.jpg" },
                new Product { Id = 14, Name = "Фломастеры", Description = "12 цветов, водные", Price = 99.00m, CategoryId = 8, ImageUrl = "/images/markers2.jpg" },
                new Product { Id = 15, Name = "Ручка с синей пастой", Description = "Простая и надёжная", Price = 19.00m, CategoryId = 1, ImageUrl = "/images/pen2.jpg" },
                new Product { Id = 16, Name = "Линейка 30 см", Description = "Пластиковая, прозрачная", Price = 25.00m, CategoryId = 6, ImageUrl = "/images/ruler.jpg" },
                new Product { Id = 17, Name = "Обложки для тетрадей", Description = "Комплект 10 шт., А5", Price = 59.00m, CategoryId = 11, ImageUrl = "/images/covers.jpg" },
                new Product { Id = 18, Name = "Пенал", Description = "На молнии, текстиль", Price = 249.00m, CategoryId = 12, ImageUrl = "/images/pencil-case.jpg" },
                new Product { Id = 19, Name = "Дырокол", Description = "Металлический, до 10 листов", Price = 179.00m, CategoryId = 6, ImageUrl = "/images/hole-punch.jpg" },
                new Product { Id = 20, Name = "Скотч канцелярский", Description = "Прозрачный, 18 мм", Price = 33.00m, CategoryId = 6, ImageUrl = "/images/tape.jpg" }
            );
        }
    }
}