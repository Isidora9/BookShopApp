using BookShop.Controllers;
using BookShop.Data;
using BookShop.Data.Migrations;
using BookShop.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Moq;

namespace BookShopTest.ControllerTests
{
    public class CartControllerTest
    {
        [Fact]
        public void CalculateDiscount_NoShippedOrders_ReturnsZero()
        {
            var user = new ApplicationUser { Id = "1"};
            var orders = new List<Order>
                {
                    new Order { UserId = user.Id, Shipped = false },
                    new Order { UserId = user.Id, Shipped = false },
                };
            var mockSet = new Mock<DbSet<Order>>();

            mockSet.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(orders.AsQueryable().Provider);
            mockSet.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orders.AsQueryable().Expression);
            mockSet.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orders.AsQueryable().ElementType);
            mockSet.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(() => orders.AsQueryable().GetEnumerator());

            var mockContext = new Mock<IApplicationDbContext>();
            mockContext.Setup(m => m.Orders).Returns(mockSet.Object);

            var mockUserManager = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(),
                null, null, null, null, null, null, null, null);

            var controller = new CartController(mockContext.Object, mockUserManager.Object);

            int discount = controller.CalculateDiscount(user);
            Assert.Equal(0, discount);
        }

        [Fact]
        public void CalculateDiscount_OneShippedOrder_Returns10()
        {
            var user = new ApplicationUser { Id = "1" };
            var orders = new List<Order>
                {
                    new Order { UserId = user.Id, Shipped = true },
                    new Order { UserId = user.Id, Shipped = false },
                };

            var mockSet = new Mock<DbSet<Order>>();

            mockSet.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(orders.AsQueryable().Provider);
            mockSet.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orders.AsQueryable().Expression);
            mockSet.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orders.AsQueryable().ElementType);
            mockSet.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(() => orders.AsQueryable().GetEnumerator());

            var mockContext = new Mock<IApplicationDbContext>();
            mockContext.Setup(m => m.Orders).Returns(mockSet.Object);

            var mockUserManager = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(),
                null, null, null, null, null, null, null, null);

            var controller = new CartController(mockContext.Object, mockUserManager.Object);

            int discount = controller.CalculateDiscount(user);
            Assert.Equal(10, discount);
        }
    }
}
