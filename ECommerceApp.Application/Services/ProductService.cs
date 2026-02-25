using ECommerceApp.Application.DTOs.ProductDtos;
using ECommerceApp.Application.Interfaces.Rebositories;
using ECommerceApp.Application.Interfaces.Services;
using ECommerceApp.Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace ECommerceApp.Application.Services
{
    public class ProductService(IGenericRebository<Product> genericRebository) : IProductService
    {
        private readonly IGenericRebository<Product> _genericRebository = genericRebository;

        public async Task<List<GetProductDetailsDto>> GetAll()
        {
            var products = await _genericRebository.GetAll().Include(p => p.Category).ToListAsync();
            var productDto = products.Adapt<List<GetProductDetailsDto>>();
            return productDto;
        }
        public async Task Add(AddProductDto newProduct)
        {
            var product = newProduct.Adapt<Product>();
            await _genericRebository.Add(product);
        }
        public async Task Delete(int id)
        {
            var deletedProduct = await _genericRebository.GetAll().FirstOrDefaultAsync(p=>p.Id==id);
            if (deletedProduct == null) return;
            await _genericRebository.Delete(deletedProduct);
        }
        public async Task Update(UpdateProductDto productDto)
        {

            var updateProduct = await _genericRebository.GetAll().FirstOrDefaultAsync(p => p.Id == productDto.Id);
            if (updateProduct == null) return;
            productDto.Adapt(updateProduct);
            await _genericRebository.Update(updateProduct);
        }
        public async Task<GetProductDetailsDto> GetProductDetails(int id)
        {
            var product = await _genericRebository.GetByIdAsync(id, includes: [product=>product.Category]);
            return product.Adapt<GetProductDetailsDto>();
        }
        public async Task<List<GetProductDetailsDto>> GetProductsByName(string name)
        {
            var products = await _genericRebository.FindAsync(p=>name.Contains(p.ProductName)).Include(p => p.Category).ToListAsync();
            var productDto = products.Adapt<List<GetProductDetailsDto>>();
            return productDto;
        }
        public async Task<List<GetProductDetailsDto>> GetProductByCategoryId(int id)
        {
            var products = await _genericRebository.FindAsync(p => p.CategoryId == id).Include(p => p.Category).ToListAsync();
            var productDto = products.Adapt<List<GetProductDetailsDto>>();
            return productDto;
        }
    }
}