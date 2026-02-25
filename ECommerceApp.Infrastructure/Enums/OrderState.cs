using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Infrastructure.Enums
{
    public enum OrderStateNow
    {
        Pending = 1,
        Processing = 2,
        Shipped = 3,
        Completed = 4,
        Cancelled = 5
    }
}
