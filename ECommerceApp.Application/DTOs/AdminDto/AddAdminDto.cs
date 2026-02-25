using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.DTOs.AdminDto
{
    public class AddAdminDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        
        public string Password { get; set; }
        [Compare("Password")]
        public string ConFirmPassword { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }
}
