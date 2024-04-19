using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookShop.Data;
using BookShop.Domain.Entities;
using NuGet.Protocol.Core.Types;
using X.PagedList;
using System.Drawing.Printing;
using BookShop.Models;

namespace BookShop.Controllers
{
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShopController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Shop
        //public async Task<IActionResult> Index(string genre, string author)
        //{
        //    ViewBag.SelectedGenre = genre;
        //    IEnumerable<Book> books;

        //    if (!string.IsNullOrEmpty(genre))
        //    {
        //        if (!string.IsNullOrEmpty(author))
        //        {
        //            books = _context.Books.Where(b => b.Genre == genre).Where(b => b.Author.Contains(author)).ToList();
        //        }
        //        else
        //        {
        //            books = _context.Books.Where(b => b.Genre == genre);
        //        }
        //    }
        //    else
        //    {
        //        if (!string.IsNullOrEmpty(author))
        //        {
        //            books = _context.Books.Where(b => b.Author.Contains(author)).ToList();
        //        }
        //        else
        //        {
        //            books = _context.Books.ToList();
        //        }
        //    }

        //    return View(books);
        //}


        public async Task<IActionResult> Index(string genre, string author, int? page, int? pageBestSelling, int? pageBestRated)
        {
            var pageNumber = page ?? 1;
            var pageSize = 5;

            var pageNumberBestSelling = pageBestSelling ?? 1;
            var pageSizeBestSelling = 5;

            var pageNumberBestRated = pageBestRated ?? 1;
            var pageSizeBestRated = 5;

            ViewBag.SelectedGenre = genre;
            IEnumerable<Book> books;

            if (!string.IsNullOrEmpty(genre))
            {
                if (!string.IsNullOrEmpty(author))
                {
                    books = _context.Books.Where(b => b.Genre == genre).Where(b => b.Author.Contains(author)).ToList();
                }
                else
                {
                    books = _context.Books.Where(b => b.Genre == genre);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(author))
                {
                    books = _context.Books.Where(b => b.Author.Contains(author)).ToList();
                }
                else
                {
                    books = _context.Books.ToList();
                }
            }
            var allBooks = await books.ToPagedListAsync(pageNumber, pageSize);
            var bestRatedBooks = await GetBestRatedBooks().ToPagedListAsync(pageNumberBestRated, pageSizeBestRated);
            var bestSellingBooks = await GetBestSellingBooks().ToPagedListAsync(pageNumberBestSelling, pageSizeBestSelling);
            var model = new BookViewModel
            {
                AllBooks = allBooks,
                BestRatedBooks = bestRatedBooks,
                BestSellingBooks = bestSellingBooks
            };

            return View(model);
        }


        private IQueryable<Book> GetBestSellingBooks()
        {
            var shippedOrders = _context.Orders.Where(o => o.Shipped == true);

            var orderItemsPerOrder = shippedOrders
                .SelectMany(o => o.OrderItems);
            var groupedOrderItems = orderItemsPerOrder
                .GroupBy(oi => oi.BookId)
                .Select(g => new
                {
                    BookId = g.Key,
                    TotalQuantitySold = g.Sum(oi => oi.Quantity)
                });

            var mostSoldBooks = groupedOrderItems
                .OrderByDescending(g => g.TotalQuantitySold)
                .Select(g => g.BookId);

            List<Book> mostSoldBooksQuery = new List<Book>();

            foreach (int id in mostSoldBooks)
            {
                mostSoldBooksQuery.Add(_context.Books.FirstOrDefault(o => o.BookId == id));
            }

            return mostSoldBooksQuery.AsQueryable();
        }


        private IQueryable<Book> GetBestRatedBooks()
        {
            var booksWithRatings = _context.Books
                .Select(book => new
                {
                    Book = book,
                    AverageRating = book.Comments != null && book.Comments.Any() ? book.Comments.Average(comment => comment.Rating) : 0
                })
                .ToList();
            booksWithRatings = booksWithRatings.OrderByDescending(x => x.AverageRating).ToList();
            //var booksWithHighestRating = booksWithRatings.Select(x => x.Book).ToList();
            var booksWithHighestRating = booksWithRatings
                .Where(x => x.AverageRating >= 4)
                .Select(x => x.Book)
                .ToList();
            return booksWithHighestRating.AsQueryable();
        }


        // GET: Shop/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Shop/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Shop/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookId,Title,Author,Description,Price,AvailableBookNum")] Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Shop/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Shop/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookId,Title,Author,Description,Price,AvailableBookNum")] Book book)
        {
            if (id != book.BookId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.BookId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Shop/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Shop/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.BookId == id);
        }
    }
}
