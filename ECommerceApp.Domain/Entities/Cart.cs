using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerceApp.Domain.Entities
{
    public class Cart : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public ICollection<CartProduct> CartProducts { get; set; }
    }
}
