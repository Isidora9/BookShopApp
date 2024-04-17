using BookShop.Controllers;
using BookShop.Data;
using BookShop.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShopTest.ControllerTests
{
    public class CommentsControllerTest
    {
        [Fact]
        public async void TestDetailsView()
        {
            //var mockContext = new Mock<ApplicationDbContext>();

            //var controller = new CommentsController(mockContext.Object);
            //var result = await controller.Details(2) as ViewResult;

            //Xunit.Assert.Equal("Details", result.ViewName);

            // Arrange
            var mockComment = new Comment { CommentId = 2 };

            // Mocking the ApplicationDbContext
            var mockContext = new Mock<IApplicationDbContext>();

            // Setup the behavior of the mocked FindAsync method
            mockContext.Setup(c => c.Comments.FindAsync(2)).ReturnsAsync(mockComment);

            // Create the CommentsController instance with the mocked context
            var controller = new CommentsController((ApplicationDbContext)mockContext.Object);

            // Act
            var result = await controller.Details(2) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Details", result.ViewName);

        }

    }
}
