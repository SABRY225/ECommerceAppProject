using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceApp.Domain.Entities;

namespace ECommerceApp.Application.DTOs.CategoryDtos
{
    public class CategoryDashboardDto
    {
        public int TotalCategories { get; set; }
        public int ActiveProducts { get; set; }
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSales { get; set; }
        public string TopPerforming { get; set; }
        public DateTime LastUpdated { get; set; }


    }
}
