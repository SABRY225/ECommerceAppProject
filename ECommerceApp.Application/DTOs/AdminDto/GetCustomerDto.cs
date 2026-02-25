using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.DTOs.AdminDto
{
    public class GetCustomerDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; } 
        public int Total_Order { get; set; }

    }
}
