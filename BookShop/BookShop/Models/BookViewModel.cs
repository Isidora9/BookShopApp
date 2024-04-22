using BookShop.Domain.Entities;
using X.PagedList;

namespace BookShop.Models
{
    public class BookViewModel
    {
        public IPagedList<Book> AllBooks { get; set; }
        public List <Book> BestRatedBooks { get; set; }
        public List <Book> BestSellingBooks { get; set; }
        //public List<Book> TopRatedBooks { get; set; }
    }
}
