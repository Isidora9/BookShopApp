using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [DisplayFormat(DataFormatString = "${0:N2}")]
        public decimal TotalPrice { get; set; }
        public bool Status { get; set; }

        public decimal CalculateTotalPrice() 
        {
            decimal totalPrice = 0;
            if (OrderItems != null)
            {
                foreach (var item in OrderItems)
                {
                    totalPrice += item.Quantity * item.Book.Price;
                }
            }

            return totalPrice;
        }
    }
}
