using System;
using System.Collections.Generic;
using System.Text;
using ECommerceApp.Application.DTOs.ProductDtos;
using ECommerceApp.Application.Interfaces.Rebositories;
using ECommerceApp.Application.Interfaces.Services;
using ECommerceApp.Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IGenericRebository<Product> _genericRebository;

        public ProductService(IGenericRebository<Product> genericRebository)
        {
            _genericRebository = genericRebository;
        }

        public IQueryable<GetProductDetailsDto> GetAll()
        {
            var products = _genericRebository.GetAll().Include(p => p.Category).ToList();
            var productDto = products.Adapt<IQueryable<GetProductDetailsDto>>();
            return productDto;

        }
        public void Add(AddProductDto newProduct)
        {
            var product = newProduct.Adapt<Product>();
            _genericRebository.Add(product);


        }

        public void Delete(int id)
        {
            var deletedProduct = _genericRebository.GetAll().FirstOrDefault(p=>p.Id==id);
            if (deletedProduct == null)
            {
                Console.WriteLine("Product Not Found ");
                return;

            }
            _genericRebository.Delete(deletedProduct);
        }


        public void Update(UpdateProductDto productDto)
        {

            var updateProduct = _genericRebository.GetAll().FirstOrDefault(p => p.Id == productDto.Id);
            if (updateProduct == null)
            {
                Console.WriteLine("Product Not Found ");
                return;
            }
            productDto.Adapt(updateProduct);

          
            _genericRebository.Update(updateProduct);
        }
    }
}