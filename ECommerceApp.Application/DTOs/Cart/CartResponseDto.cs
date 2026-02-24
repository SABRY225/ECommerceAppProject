using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.DTOs.Cart
{
    public class CartResponseDto
    {
        public int UserId { get; set; }
        public List<CartItemDto> Items { get; set; }
    }
}
