using BookShop.Data;
using BookShop.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Controllers
{
    public class CartController : Controller
    {
        private static List<OrderItem> _cartItems = new List<OrderItem>();

        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_cartItems);
        }

        [HttpPost]
        public IActionResult AddToCart(int bookId, int quantity)
        {
            var book = _context.Books.FirstOrDefault(b => b.BookId == bookId);
            if (book == null)
            {
                return NotFound();
            }

            if (quantity <= book.AvailableBookNum)
            {
                // Check if the book is already in the cart
                var existingCartItem = _cartItems.FirstOrDefault(item => item.BookId == bookId);
                if (existingCartItem != null)
                {
                    existingCartItem.Quantity += quantity;
                }
                else
                {
                    // If not, add it to the cart
                    _cartItems.Add(new OrderItem { BookId = bookId, Quantity = quantity, Book = book });
                }
            }
            else 
            {
                TempData["ErrorMessage"] = $"There are only {book.AvailableBookNum} books available.";
                //ModelState.AddModelError("quantity", $"There are only {book.AvailableBookNum} books available.");
                //return RedirectToAction("Index");
            }

            

            return RedirectToAction("Index", "Cart");
        }
    }
}
