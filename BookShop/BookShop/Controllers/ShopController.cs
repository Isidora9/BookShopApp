﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookShop.Data;
using BookShop.Domain.Entities;
using NuGet.Protocol.Core.Types;

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
        public async Task<IActionResult> Index(string genre, string author, string data)
        {
            ViewBag.NumOfAddedBooks = data;
            //int numberOfAddedBooks = 0;
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
                    // If no genre is selected, get all books
                    books = _context.Books.ToList();
                }
            }

            return View(books);
            //return View(await _context.Books.ToListAsync());
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

        //public IActionResult FilterByGenre(string genre)
        //{
        //    List<Book> filteredBooks = _context.Books.Where(b => b.Genre == genre).ToList();
        //    return View(filteredBooks);
        //}

        //private List<Book> FindBooksByGenre(string genre)
        //{
        //    List<Book> filteredBooks = _context.Books.Where(b => b.Genre == genre).ToList();
        //    return filteredBooks;
        //}

        //private List<Book> FindBooksByAuthor(List<Book> books, string author)
        //{
        //    List<Book> filteredBooks = books.Where(b => b.Author == author).ToList();
        //    return filteredBooks;
        //}

        //private int CalculateNumberOfAddedBooks() 
        //{

        //}

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.BookId == id);
        }
    }
}
