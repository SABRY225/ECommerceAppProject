using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.DTOs.Cart
{
    public class AddProductToCartDto
    {
        public int ProductId { get; set; }
        public int CustomerId {  get; set; }
    }
}
