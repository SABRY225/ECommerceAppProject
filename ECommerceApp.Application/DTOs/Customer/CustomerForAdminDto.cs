using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.DTOs.Customer
{
    public class CustomerForAdminDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public int TotalOrders { get; set; }
        public DateTime JoinDate { get; set; }
    }
}
