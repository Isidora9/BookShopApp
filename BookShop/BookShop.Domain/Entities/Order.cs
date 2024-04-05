using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Domain.Entities
{
    public class Order
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public ICollection<OrderItem>? OrderItems { get; set; }
        public decimal TotalPrice { get; set; }
        public bool Status { get; set; }
        public ApplicationUser User { get; set; }
    }
}
