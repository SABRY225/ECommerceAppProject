using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.DTOs.CategoryDtos
{
  public class AddCategoryDto
    {

        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
    }
}
