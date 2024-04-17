using BookShop.Controllers;
using BookShop.Data;
using BookShop.Data.Migrations;
using BookShop.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace BookShopTest.ControllerTests
{
    public class CartControllerTests
    {
        [Fact]
        public void CalculateDiscount_NoShippedOrders_ReturnsZero()
        {
            var user = new ApplicationUser { Id = "1"};
            //var orders = new List<Order>();
            //var contextMock = new Mock<ApplicationDbContext>();
            //contextMock.Setup(c => c.Orders).Returns(mockSet.Object);

            var mockSet = new Mock<DbSet<Order>>();
            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(m => m.Orders).Returns(mockSet.Object);

            var mockUserManager = new Mock<UserManager<ApplicationUser>>();


            var controller = new CartController(mockContext.Object, mockUserManager.Object);

            int discount = controller.CalculateDiscount(user);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Equals(0, discount);

        }
    }
}
