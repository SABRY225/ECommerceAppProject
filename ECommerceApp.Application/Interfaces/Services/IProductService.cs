using System;
using System.Collections.Generic;
using System.Text;
using ECommerceApp.Application.DTOs.CategoryDtos;
using ECommerceApp.Application.DTOs.ProductDtos;

namespace ECommerceApp.Application.Interfaces.Services
{
   public interface IProductService
    {
        public Task<List<GetProductDetailsDto>> GetAll();
        public Task<GetProductDetailsDto> GetProductDetails(int id);
        public Task<List<GetProductDetailsDto>> GetProductsByName(string name);
        public Task<List<GetProductDetailsDto>> GetProductByCategoryId(int id);
        public Task Add(AddProductDto dto);
        public Task Update(UpdateProductDto dto);
        public Task Delete(int id);
    }
}
