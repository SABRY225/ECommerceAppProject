using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerceApp.Infrastructure.Enums
{
    public enum OrderState
    {
        Pending = 1,
        Processing = 2,
        Shipped = 3,
        Completed = 4,
        Cancelled = 5
    }
}
