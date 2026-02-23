using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.DTOs.ProductDtos
{
    public class UpdateProductDto
    {

        public int Id { get; set; }

        public string ProductName { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public int StockQuantity { get; set; }
        public decimal Price { get; set; }

        public int CategoryId { get; set; }
    }
}
