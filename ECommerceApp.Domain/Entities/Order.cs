using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerceApp.Domain.Entities
{
    public class Order : BaseEntity
    {
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string State { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
