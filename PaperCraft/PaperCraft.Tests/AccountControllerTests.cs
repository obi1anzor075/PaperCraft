using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using PaperCraft;
using PaperCraft.Controllers;
using PaperCraft.Data;
using PaperCraft.Models;
using PaperCraft.Models.PaperCraft.Models;
using PaperCraft.Services;
using PaperCraft.ViewModels;
using Xunit;

namespace PaperCraft.Tests.Controllers
{
    public class AccountControllerTests
    {
        private readonly Mock<UserManager<AppUser>> _userMgr;
        private readonly Mock<SignInManager<AppUser>> _signInMgr;
        private readonly Mock<AppDbContext> _context;
        private readonly Mock<IUserActivityService> _activityService;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            // Настраиваем Mock<UserManager<AppUser>>
            var userStore = new Mock<IUserStore<AppUser>>();
            _userMgr = new Mock<UserManager<AppUser>>(userStore.Object, null, null, null, null, null, null, null, null);

            // Настраиваем Mock<SignInManager<AppUser>>
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(a => a.HttpContext).Returns(new DefaultHttpContext());
            var userClaimsFactory = new Mock<IUserClaimsPrincipalFactory<AppUser>>();
            _signInMgr = new Mock<SignInManager<AppUser>>(
                _userMgr.Object,
                httpContextAccessor.Object,
                userClaimsFactory.Object,
                null, null, null, null
            );

            _context = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            _activityService = new Mock<IUserActivityService>();

            _controller = new AccountController(
                _userMgr.Object,
                _signInMgr.Object,
                _context.Object,
                _activityService.Object
            );

            // Чтобы в JSON-ответах не было проблем с сериализацией
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Fact]
        public void Register_Get_ReturnsPartialView()
        {
            // Act
            var result = _controller.Register();

            // Assert
            var view = Assert.IsType<PartialViewResult>(result);
            Assert.Equal("_RegisterPartial", view.ViewName);
        }

        [Fact]
        public async Task Register_Post_InvalidModelState_ReturnsPartialView()
        {
            // Arrange
            _controller.ModelState.AddModelError("Email", "Required");
            var vm = new RegisterViewModel();

            // Act
            var result = await _controller.Register(vm);

            // Assert
            var view = Assert.IsType<PartialViewResult>(result);
            Assert.Equal("_RegisterPartial", view.ViewName);
            Assert.Same(vm, view.Model);
        }

        [Fact]
        public void Login_Get_ReturnsPartialView()
        {
            // Act
            var result = _controller.Login();

            // Assert
            var view = Assert.IsType<PartialViewResult>(result);
            Assert.Equal("_LoginPartial", view.ViewName);
        }

        [Fact]
        public async Task Login_Post_Success_ReturnsJsonSuccess()
        {
            // Arrange
            var vm = new LoginViewModel { Email = "a@b.com", Password = "pwd", RememberMe = true };
            _signInMgr
              .Setup(s => s.PasswordSignInAsync(vm.Email, vm.Password, vm.RememberMe, false))
              .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
            _userMgr
              .Setup(u => u.FindByEmailAsync(vm.Email))
              .ReturnsAsync(new AppUser { Id = "1", Email = vm.Email });

            // Act
            var result = await _controller.Login(vm);

            // Assert
            var json = Assert.IsType<JsonResult>(result);
            // Получаем свойство "success" через отражение
            var val = json.Value;
            var prop = val.GetType().GetProperty("success");
            Assert.NotNull(prop);
            var success = (bool)prop.GetValue(val);
            Assert.True(success);

            _activityService.Verify(a => a.LogActivityAsync(
                "1", "Вход в систему", It.Is<string>(s => s.StartsWith("IP:")),
                It.IsAny<string>(), It.IsAny<string>(), ActivityType.Login),
                Times.Once);
        }


        [Fact]
        public async Task Login_Post_Failure_ReturnsPartialViewWithError()
        {
            // Arrange
            var vm = new LoginViewModel { Email = "x", Password = "y", RememberMe = false };
            _signInMgr.Setup(s => s.PasswordSignInAsync(vm.Email, vm.Password, false, false))
                      .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            // Act
            var result = await _controller.Login(vm);

            // Assert
            var view = Assert.IsType<PartialViewResult>(result);
            Assert.False(_controller.ModelState.IsValid);
            Assert.Contains("Неверные учетные данные", _controller.ModelState[string.Empty].Errors[0].ErrorMessage);
        }
    }

    // Вспомогательный метод для создания DbSet из списка
    public static class MoqExtensions
    {
        public static DbSet<T> ReturnsDbSet<T>(this Mock<IQueryable<T>> mock, IList<T> list) where T : class
        {
            var queryable = list.AsQueryable();
            mock.Setup(m => m.Provider).Returns(queryable.Provider);
            mock.Setup(m => m.Expression).Returns(queryable.Expression);
            mock.Setup(m => m.ElementType).Returns(queryable.ElementType);
            mock.Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            return mock.As<DbSet<T>>().Object;
        }

        public static DbSet<T> ReturnsDbSet<T>(this Mock<DbSet<T>> mock, IList<T> list) where T : class
        {
            var queryable = list.AsQueryable();
            mock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            return mock.Object;
        }

        public static DbSet<T> ReturnsDbSet<T>(this Mock<UserManager<AppUser>> mock, IList<AppUser> list) where T : class
        {
            // Для _userMgr.Users
            var users = new Mock<DbSet<AppUser>>();
            users.ReturnsDbSet(list);
            mock.Setup(u => u.Users).Returns(users.Object);
            return users.Object as DbSet<T>;
        }
    }
}
