using ECommerceApp.Application.DTOs.ProductDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.DTOs.Order
{
    public class GetOrderDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string State { get; set; }
        public int ProductsCount { get; set; }
        public List<OrderProductDto> OrderProducts { get; set; }
    }
}
