using BookShop.Controllers;
using BookShop.Data;
using BookShop.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Linq.Expressions;
using static System.Reflection.Metadata.BlobBuilder;

namespace BookShopTest.ControllerTests
{
    public class BooksControllerTest
    {
        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfBooks()
        {
            // Arrange
            var mockUserManager = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
            var mockSet = new Mock<DbSet<Book>>();
            var books = new List<Book>
            {
                new Book { BookId = 1, Title = "Dune" },
                new Book { BookId = 2, Title = "Harry Potter" },
            };

            mockSet.As<IAsyncEnumerable<Book>>()
            .Setup(m => m.GetAsyncEnumerator(CancellationToken.None))
            .Returns(new TestAsyncEnumerator<Book>(books.GetEnumerator()));

            mockSet.As<IQueryable<Book>>().Setup(m => m.Provider).Returns(books.AsQueryable().Provider);
            mockSet.As<IQueryable<Book>>().Setup(m => m.Expression).Returns(books.AsQueryable().Expression);
            mockSet.As<IQueryable<Book>>().Setup(m => m.ElementType).Returns(books.AsQueryable().ElementType);
            mockSet.As<IQueryable<Book>>().Setup(m => m.GetEnumerator()).Returns(() => books.AsQueryable().GetEnumerator());

            var mockContext = new Mock<IApplicationDbContext>();
            mockContext.Setup(m => m.Books).Returns(mockSet.Object);

            var controller = new BooksController(mockContext.Object, mockUserManager.Object);
            var result = await controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Book>>(viewResult.ViewData.Model);
            Assert.Equal(books.Count, model.Count());
        }

        [Fact]
        public async Task Create_WithValidModelState_ReturnsRedirectToActionResult()
        {
            // Arrange
            var mockUserManager = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            var book = new Book { BookId = 1, Title = "Dune" };
            var mockSet = new Mock<DbSet<Book>>();
            var mockContext = new Mock<IApplicationDbContext>();
            mockContext.Setup(m => m.Books).Returns(mockSet.Object);
            var controller = new BooksController(mockContext.Object, mockUserManager.Object);

            // Act
            var result = await controller.Create(book);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Create_WithInvalidModelState_ReturnsViewResult()
        {
            // Arrange
            var mockUserManager = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            var book = new Book { BookId = 1, Genre = "Fantasy" };
            var mockSet = new Mock<DbSet<Book>>();
            var mockContext = new Mock<IApplicationDbContext>();
            mockContext.Setup(m => m.Books).Returns(mockSet.Object);
            var controller = new BooksController(mockContext.Object, mockUserManager.Object);
            controller.ModelState.AddModelError("Title", "Title is required");

            // Act
            var result = await controller.Create(book);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(book, viewResult.Model); // Ensure the model is passed back to the view
        }

        [Fact]
        public async Task Edit_WithValidModelAndExistingBook_ReturnsRedirectToActionResult()
        {
            // Arrange
            var mockUserManager = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            int id = 1;
            var book = new Book { BookId = id, Title = "Dune" };
            var mockSet = new Mock<DbSet<Book>>();
            var mockContext = new Mock<IApplicationDbContext>();
            mockContext.Setup(m => m.Books).Returns(mockSet.Object);
            var controller = new BooksController(mockContext.Object, mockUserManager.Object);

            // Act
            var result = await controller.Edit(id, book);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Edit_WithInvalidModel_ReturnsViewResult() 
        {
            // Arrange
            var mockUserManager = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            int id = 1;
            var book = new Book { BookId = id, Description = "Description" };
            var mockSet = new Mock<DbSet<Book>>();
            var mockContext = new Mock<IApplicationDbContext>();
            mockContext.Setup(m => m.Books).Returns(mockSet.Object);
            var controller = new BooksController(mockContext.Object, mockUserManager.Object);
            controller.ModelState.AddModelError("Title", "Title is required");
            // Act
            var result = await controller.Edit(id, book);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(book, viewResult.Model);
        }

        [Fact]
        public async Task Edit_WithInvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            var mockUserManager = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            int id = 1;
            var book = new Book { BookId = id, Title = "Dune" };
            var mockSet = new Mock<DbSet<Book>>();
            var mockContext = new Mock<IApplicationDbContext>();
            mockContext.Setup(m => m.Books).Returns(mockSet.Object);
            var controller = new BooksController(mockContext.Object, mockUserManager.Object);

            // Act
            var result = await controller.Edit(2, book);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteConfirmed_WithExistingBook_ReturnsRedirectToActionResult()
        {
            // Arrange
            var mockUserManager = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            int id = 1;
            var book = new Book { BookId = id, Title = "Dune" };
            var mockSet = new Mock<DbSet<Book>>();
            var mockContext = new Mock<IApplicationDbContext>();
            mockContext.Setup(m => m.Books.FindAsync(id)).ReturnsAsync(book);
            var controller = new BooksController(mockContext.Object, mockUserManager.Object);

            // Act
            var result = await controller.DeleteConfirmed(id);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Delete_WithExistingBookId_ReturnsViewResultWithBook()
        {
            // Arrange
            var mockUserManager = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            int id = 1; // Specify an existing book id
            var book = new Book { BookId = id, Title = "Dune" };
            var books = new List<Book> { book };
            var mockSet = new Mock<DbSet<Book>>();
            //mockSet.Setup(m => m.FindAsync(id)).ReturnsAsync(book);

            mockSet.As<IQueryable<Book>>().Setup(m => m.Provider).Returns(books.AsQueryable().Provider);
            mockSet.As<IQueryable<Book>>().Setup(m => m.Expression).Returns(books.AsQueryable().Expression);
            mockSet.As<IQueryable<Book>>().Setup(m => m.ElementType).Returns(books.AsQueryable().ElementType);
            mockSet.As<IQueryable<Book>>().Setup(m => m.GetEnumerator()).Returns(books.AsQueryable().GetEnumerator());

            var mockContext = new Mock<IApplicationDbContext>();
            mockContext.Setup(m => m.Books).Returns(mockSet.Object);
            var controller = new BooksController(mockContext.Object, mockUserManager.Object);

            // Act
            var result = await controller.Delete(id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Book>(viewResult.Model);
            Assert.Equal(book, model);
        }
    }
}
