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

        private readonly IApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(IApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            int discount = 0;
            var user = await _userManager.GetUserAsync(this.User);

            var activeOrder = _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(o => o.Book)
            .FirstOrDefault(o => o.UserId == user.Id && o.Shipped == false);
            if (activeOrder != null)
            {
                activeOrder.TotalPrice = activeOrder.CalculateTotalPrice();
                await _context.SaveChangesAsync();
            }

            if (activeOrder == null)
            {
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

            discount = CalculateDiscount(user);
            ViewBag.LoyaltyDiscount = discount;

            return View(activeOrder);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int bookId, int quantity)
        {
            var user = await _userManager.GetUserAsync(this.User);

            var activeOrder = _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefault(o => o.UserId == user.Id && o.Shipped == false);

            if (activeOrder == null)
            {
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
                var existingCartItem = activeOrder.OrderItems.FirstOrDefault(item => item.BookId == bookId);
                if (existingCartItem != null)
                {
                    existingCartItem.Quantity += quantity;
                }
                else
                {
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
                if (quantityDifference > 0)
                {
                    book.AvailableBookNum -= quantityDifference;
                }
                else
                {
                    book.AvailableBookNum -= quantityDifference;
                }
                await _context.SaveChangesAsync();
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

        public async Task<IActionResult> OrderSuccess() 
        {
            var user = await _userManager.GetUserAsync(this.User);

            var activeOrder = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(o => o.Book)
                .FirstOrDefault(o => o.UserId == user.Id && o.Shipped == false);
            if (activeOrder != null)
            {
                activeOrder.Shipped = true;
                activeOrder.TotalPrice = activeOrder.CalculateTotalPrice();
                await _context.SaveChangesAsync();
            }
            return View();
        }

        public int CalculateDiscount(ApplicationUser user)
        {
            int disount = 0;
            var shippedOrders = _context.Orders
                .Where(o => o.UserId == user.Id && o.Shipped == true);

            if (shippedOrders != null)
            {
                if (shippedOrders.Count() == 1)
                {
                    disount = 10;
                }
                else if (shippedOrders.Count() == 2)
                {
                    disount = 15;
                }
                else if (shippedOrders.Count() >= 3)
                {
                    disount = 20;
                }
            }
            return disount;
        }
    }
}
