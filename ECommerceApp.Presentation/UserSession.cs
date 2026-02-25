using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Presentation
{
    public static class UserSession
    {
        public static int CustomerId { get; set; }
        public static string CustomerName { get; set; }
        public static string Email { get; set; }

        public static void Logout()
        {
            CustomerId = 0;
            CustomerName = null;
            Email = null;
        }
    }
}
