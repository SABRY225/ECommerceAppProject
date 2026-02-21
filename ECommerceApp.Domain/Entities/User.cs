using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerceApp.Domain.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        public Cart Cart { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
