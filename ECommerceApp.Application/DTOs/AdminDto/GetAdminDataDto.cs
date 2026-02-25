using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.DTOs.AdminDto
{
    public  class GetAdminDataDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime Lsat_login { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
