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
            var mockComment = new Comment { CommentId = 2 };

            var mockContext = new Mock<IApplicationDbContext>();

            mockContext.Setup(c => c.Comments.FindAsync(2)).ReturnsAsync(mockComment);

            var controller = new CommentsController((ApplicationDbContext)mockContext.Object);

            var result = await controller.Details(2) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal("Details", result.ViewName);

        }

    }
}
