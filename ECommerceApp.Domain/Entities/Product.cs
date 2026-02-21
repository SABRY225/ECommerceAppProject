using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerceApp.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public int StockQuantity { get; set; }
        public decimal Price { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public ICollection<OrderProduct> OrderProducts { get; set; }
        public ICollection<CartProduct> CartProducts { get; set; }
    }
}
