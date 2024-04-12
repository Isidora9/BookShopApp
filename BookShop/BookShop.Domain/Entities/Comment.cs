using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Domain.Entities
{
    public class Comment
    {
        public int CommentId { get; set; }
        public string UserId { get; set; }
        public int BookId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public ApplicationUser User { get; set; }
        public Book Book { get; set; }
    }
}
