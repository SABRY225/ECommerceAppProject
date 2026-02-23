using System;
using System.Collections.Generic;
using System.Text;
using ECommerceApp.Application.DTOs.ProductDtos;
using ECommerceApp.Application.Interfaces.Rebositories;
using ECommerceApp.Application.Interfaces.Services;
using ECommerceApp.Domain.Entities;

namespace ECommerceApp.Application.Services
{
   public class ProductService : IProductService
    {
        private readonly IGenericRebository<Product> _genericRebository;
        public ProductService(IGenericRebository<Product> genericRebository)
        {
            _genericRebository = genericRebository;
        }

        public void Add(AddProductDto dto)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<GetProductDetailsDto> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Update(UpdateProductDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
