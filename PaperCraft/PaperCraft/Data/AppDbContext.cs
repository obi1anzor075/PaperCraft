using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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
        public DbSet<Review> Reviews { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Configure SQL Server for design-time and fallback
                optionsBuilder
                    .UseSqlServer("Server=localhost;Database=PaperCraft;Trusted_Connection=True;MultipleActiveResultSets=true;")
                    .ConfigureWarnings(warnings =>
                        warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure default values via SQL Server
            modelBuilder.Entity<AppUser>()
                .Property(u => u.RegistrationDate)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<Category>()
                .Property(c => c.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<Product>()
                .Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<Order>()
                .Property(o => o.OrderDate)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<Review>()
                .Property(r => r.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<UserActivity>()
                .Property(ua => ua.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Configure relations
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

            // Decimal precision
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasPrecision(18, 2);
            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.Price)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            // Seed data with fixed dates
            var seedDate = new DateTime(2025, 5, 1, 0, 0, 0, DateTimeKind.Utc);

            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Ручки", Description = "Шариковые, гелевые ручки, маркеры", Icon = "fas fa-pen", CreatedAt = seedDate },
                new Category { Id = 2, Name = "Тетради", Description = "Тетради различных форматов", Icon = "fas fa-book", CreatedAt = seedDate },
                new Category { Id = 3, Name = "Маркер", Description = "Маркеры для выделения и письма", Icon = "fas fa-highlighter", CreatedAt = seedDate },
                new Category { Id = 4, Name = "Карандаши", Description = "Простые и цветные карандаши", Icon = "fas fa-pencil-alt", CreatedAt = seedDate },
                new Category { Id = 5, Name = "Блокноты", Description = "Блокноты и записные книжки", Icon = "fas fa-sticky-note", CreatedAt = seedDate },
                new Category { Id = 6, Name = "Офис", Description = "Офисные принадлежности", Icon = "fas fa-briefcase", CreatedAt = seedDate },
                new Category { Id = 7, Name = "Папки", Description = "Папки и регистраторы", Icon = "fas fa-folder", CreatedAt = seedDate },
                new Category { Id = 8, Name = "Творчество", Description = "Товары для творчества", Icon = "fas fa-palette", CreatedAt = seedDate },
                new Category { Id = 9, Name = "Канцтовары", Description = "Различные канцелярские товары", Icon = "fas fa-paperclip", CreatedAt = seedDate },
                new Category { Id = 10, Name = "Клей", Description = "Клеящие материалы", Icon = "fas fa-prescription-bottle", CreatedAt = seedDate },
                new Category { Id = 11, Name = "Обложки", Description = "Обложки для тетрадей и книг", Icon = "fas fa-shield-alt", CreatedAt = seedDate },
                new Category { Id = 12, Name = "Аксессуары", Description = "Школьные и офисные аксессуары", Icon = "fas fa-tools", CreatedAt = seedDate }
            );

            // Seed Products
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Гелевая ручка", Description = "Гладкое письмо, чёрные чернила", Price = 49.99m, CategoryId = 1, ImageUrl = "https://s3.ibta.ru/goods/141180/42e90c64e373e4289de2aee5972c951a_l.jpg", IsActive = true, CreatedAt = seedDate },
                new Product { Id = 2, Name = "Тетрадь 96 л.", Description = "Клетка, обложка с рисунком", Price = 89.00m, CategoryId = 2, ImageUrl = "https://s3.ibta.ru/goods/402797/6231df38054d848d9842cb5c48cee431_l.jpg", IsActive = true, CreatedAt = seedDate },
                new Product { Id = 3, Name = "Маркеры для выделения", Description = "Набор из 6 неоновых цветов", Price = 149.50m, CategoryId = 3, ImageUrl = "https://s3.ibta.ru/goods/150491/3d1b800b86fca20ec4720ec41354bb62_l.jpg", IsActive = true, CreatedAt = seedDate },
                new Product { Id = 4, Name = "Карандаш механический", Description = "С ластиком, 0.5 мм", Price = 69.00m, CategoryId = 4, ImageUrl = "https://s3.ibta.ru/goods/180286/6b21f01f60312703ea352df2bd608752_l.jpg", IsActive = true, CreatedAt = seedDate },
                new Product { Id = 5, Name = "Блокнот A5", Description = "На спирали, 120 стр.", Price = 129.00m, CategoryId = 5, ImageUrl = "https://s3.ibta.ru/goods/111274/74e39a55982c7b2c92a94b0c5a711795_l.jpg", IsActive = true, CreatedAt = seedDate },
                new Product { Id = 6, Name = "Скрепки", Description = "Металлические, 100 шт.", Price = 35.00m, CategoryId = 6, ImageUrl = "https://s3.ibta.ru/goods/220012/6d2547ea2cde13a2d7dacff0ce8504e8_l.jpg", IsActive = true, CreatedAt = seedDate },
                new Product { Id = 7, Name = "Папка-регистратор", Description = "Формат A4, синяя", Price = 199.00m, CategoryId = 7, ImageUrl = "https://s3.ibta.ru/goods/225753/f50219f336419e2578c5ab08050257b4_l.jpg", IsActive = true, CreatedAt = seedDate },
                new Product { Id = 8, Name = "Калькулятор", Description = "12-разрядный дисплей", Price = 499.00m, CategoryId = 6, ImageUrl = "https://s3.ibta.ru/goods/250462/697149d326a54f6e6ba1882d76ee3994_l.jpg", IsActive = true, CreatedAt = seedDate },
                new Product { Id = 9, Name = "Цветные карандаши", Description = "Набор 24 цвета", Price = 189.90m, CategoryId = 8, ImageUrl = "https://s3.ibta.ru/goods/181690/9df61fc824c0f8facb7d19251e6061e4_l.jpg", IsActive = true, CreatedAt = seedDate }
            );
        }
    }
}