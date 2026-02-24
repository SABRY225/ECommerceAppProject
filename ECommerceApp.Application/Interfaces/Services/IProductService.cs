using System;
using System.Collections.Generic;
using System.Text;
using ECommerceApp.Application.DTOs.CategoryDtos;
using ECommerceApp.Application.DTOs.ProductDtos;

namespace ECommerceApp.Application.Interfaces.Services
{
   public interface IProductService
    {
        public IQueryable<GetProductDetailsDto> GetAll();
        public void Add(AddProductDto dto);
        public void Update(UpdateProductDto dto);
        public void Delete(int id);
    }
}
