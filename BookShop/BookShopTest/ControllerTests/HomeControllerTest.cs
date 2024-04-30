using BookShop.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShopTest.ControllerTests
{
    public class HomeControllerTest
    {
        [Fact]
        public void Test_Index_ReturnsViewName() 
        {
            var loggerMock = new Mock<ILogger<HomeController>>();
            var controller = new HomeController(loggerMock.Object);
            var result = controller.Index() as ViewResult;
            Assert.Equal("Index", result?.ViewName);
        }
    }
}
