using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerceApp.Domain.Entities
{
    public class CartProduct
    {
        public int CartId { get; set; }
        public Cart Cart { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }
        public DateTime Date { get; set; }
    }
}
