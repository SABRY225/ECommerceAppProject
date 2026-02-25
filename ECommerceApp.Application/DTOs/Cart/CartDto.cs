using ECommerceApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.DTOs.Cart
{
    public class CartDto
    {
        public int UserId { get; set; }
        public List<CartProductDto> CartProducts { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
