using BookShop.Data;
using BookShop.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BookShop.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        //private static List<OrderItem> _cartItems = new List<OrderItem>();

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            int cartItemCount = 0;
            var user = await _userManager.GetUserAsync(this.User);

            var activeOrder = _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(o => o.Book)
            .FirstOrDefault(o => o.UserId == user.Id);
            if (activeOrder != null)
            {
                activeOrder.TotalPrice = activeOrder.CalculateTotalPrice();
            }

            if (activeOrder == null)
            {
                // If no active order exists, create a new one
                activeOrder = new Order
                {
                    UserId = user.Id,
                    OrderDate = DateTime.Now,
                    OrderItems = new List<OrderItem>(),
                };
                activeOrder.TotalPrice = activeOrder.CalculateTotalPrice();
                _context.Orders.Add(activeOrder);
                await _context.SaveChangesAsync();
            }
            foreach (var item in activeOrder.OrderItems)
            {
                cartItemCount += item.Quantity;
            }
            ViewBag.CartItemCount = cartItemCount;
            return View(activeOrder);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int bookId, int quantity)
        {
            var user = await _userManager.GetUserAsync(this.User);

            // Find the user's active order (if any)
            var activeOrder = _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefault(o => o.UserId == user.Id && o.Status == false);

            if (activeOrder == null)
            {
                // If no active order exists, create a new one
                activeOrder = new Order
                {
                    UserId = user.Id,
                    OrderDate = DateTime.Now,
                    OrderItems = new List<OrderItem>()
                };
                _context.Orders.Add(activeOrder);
                await _context.SaveChangesAsync();
            }



            var book = _context.Books.FirstOrDefault(b => b.BookId == bookId);
            if (book == null)
            {
                return NotFound();
            }

            if (quantity <= book.AvailableBookNum)
            {
                // Check if the book is already in the cart
                var existingCartItem = activeOrder.OrderItems.FirstOrDefault(item => item.BookId == bookId);
                if (existingCartItem != null)
                {
                    existingCartItem.Quantity += quantity;
                }
                else
                {
                    // If not, add it to the cart
                    activeOrder.OrderItems.Add(new OrderItem { BookId = bookId, Quantity = quantity, Book = book });
                }
                book.AvailableBookNum -= quantity;
                await _context.SaveChangesAsync();
            }
            else
            {
                TempData["ErrorMessage"] = $"There are only {book.AvailableBookNum} books available.";
                return RedirectToAction("Index", "Shop", new { id = bookId });
            }

            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int orderItemId)
        {
            var orderItem = await _context.OrderItems.FindAsync(orderItemId);
            if (orderItem == null)
            {
                return NotFound();
            }
            await _context.Entry(orderItem).Reference(o => o.Book).LoadAsync();
            var book = orderItem.Book;
            book.AvailableBookNum += orderItem.Quantity;
            _context.OrderItems.Remove(orderItem);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int itemId, int quantity)
        {
            var orderItem = await _context.OrderItems.FindAsync(itemId);
            if (orderItem == null)
            {
                return NotFound();
            }
            var oldQuantity = orderItem.Quantity;
            orderItem.Quantity = quantity;
            await _context.Entry(orderItem).Reference(o => o.Book).LoadAsync();
            var book = orderItem.Book;
            var quantityDifference = quantity - oldQuantity;
            if (book.AvailableBookNum - quantityDifference >= 0)
            {
                //book.AvailableBookNum -= quantity;
                if (quantityDifference > 0)
                {
                    book.AvailableBookNum -= quantityDifference;
                }
                else
                {
                    book.AvailableBookNum -= quantityDifference;
                }
                await _context.SaveChangesAsync();
                //return Ok("Quantity updated successfully.");
                return RedirectToAction("Index", "Cart");
            }
            else
            {
                TempData["ErrorMessage"] = $"There are only {book.AvailableBookNum} books available.";
                return RedirectToAction("Index", "Cart");
            }
        }

        public IActionResult CartItemCount() 
        {
            
            return View();
        }

        public IActionResult OrderSuccess() 
        {
            return View();
        }
    }
}
