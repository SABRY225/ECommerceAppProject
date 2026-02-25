using ECommerceApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.DTOs.Order
{
    public class OrderProductDto
    {
        public int Quantity { get; set; }
        public decimal PriceAtPurchase { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }

    }
}
