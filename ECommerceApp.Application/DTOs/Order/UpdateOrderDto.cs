using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.DTOs.Order
{
    public class UpdateOrderDto
    {
        public int Id { get; set; }
        public string State { get; set; }
    }
}
