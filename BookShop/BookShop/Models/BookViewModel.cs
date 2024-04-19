using BookShop.Domain.Entities;
using X.PagedList;

namespace BookShop.Models
{
    public class BookViewModel
    {
        public IPagedList<Book> AllBooks { get; set; }
        public IPagedList<Book> BestRatedBooks { get; set; }
        public IPagedList<Book> BestSellingBooks { get; set; }
    }
}
