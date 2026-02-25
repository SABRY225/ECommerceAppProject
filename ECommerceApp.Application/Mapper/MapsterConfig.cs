using ECommerceApp.Application.DTOs.Cart;
using ECommerceApp.Application.DTOs.CategoryDtos;
using ECommerceApp.Application.DTOs.Order;
using ECommerceApp.Domain.Entities;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Mapper
{
    public static class MapsterConfig
    {
        public static void Config()
        {
            //   TypeAdapterConfig<Category, GetCategoryDto>
            //.NewConfig()
            //.Map(des => des.Id, src => src.Id);

            TypeAdapterConfig<Category, GetCategoryDto>.NewConfig();

            TypeAdapterConfig<Category, AddCategoryDto>.NewConfig();

            TypeAdapterConfig<Category, UpdateCategoryDto>.NewConfig();
            TypeAdapterConfig<Category, CategoryDashboardDto>.NewConfig();



            TypeAdapterConfig<OrderProduct, OrderProductDto>.NewConfig()
                .Map(dest => dest.ImagePath, src => src.Product.ImagePath)
                .Map(dest => dest.ProductName, src => src.Product.ProductName)
                .Map(dest => dest.Description, src => src.Product.Description);

            TypeAdapterConfig<Order,GetOrderDto>.NewConfig()
                .Map(dest=>dest.ProductsCount ,src=>src.OrderProducts.Count)
                .Map(dest=>dest.OrderProducts, src=>src.OrderProducts);



            TypeAdapterConfig<CartProduct, CartProductDto>
                .NewConfig()
                .Map(dest => dest.ProductId, src => src.ProductId)
                .Map(dest => dest.ProductName, src => src.Product.ProductName)
                .Map(dest => dest.Description, src => src.Product.Description)
                .Map(dest => dest.ImagePath, src => src.Product.ImagePath)
                .Map(dest => dest.Price, src => src.Product.Price)
                .Map(dest => dest.Quantity, src => src.Quantity);

            TypeAdapterConfig<Cart, CartDto>
                .NewConfig()
                .Map(dest => dest.UserId, src => src.UserId)
                .Map(dest => dest.CartProducts, src => src.CartProducts);
        }

    }
}
