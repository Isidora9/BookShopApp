using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Domain.Entities
{
    public class Book
    {
        public int BookId { get; set; }
        [Required]
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        [Display(Name = "Image Url")]
        public string ImgUrl { get; set; }
        public string Description { get; set; }
        [DisplayFormat(DataFormatString = "${0:N2}")]
        public decimal Price { get; set; }
        [Display(Name = "Available")]
        public int AvailableBookNum { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public double AverageRating
        {
            get
            {
                if (Comments == null || Comments.Count == 0)
                    return 0;
                return Math.Round(Comments.Average(c => c.Rating), 1);
            }
        }
    }
}
