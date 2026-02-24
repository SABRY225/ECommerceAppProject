using ECommerceApp.Application.DTOs.Cart;
using ECommerceApp.Domain.Entities;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Mapping.CartMapping
{
    public static class CartMapping
    {
        public static void RegisterMappings()
        {

            TypeAdapterConfig<CartItemRequest, CartProduct>
                .NewConfig()
                .Map(dest => dest.ProductId, src => src.ProductId)
                .Map(dest => dest.Quantity, src => src.Quantity)
                .Map(dest => dest.Date, src => DateTime.Now);

            TypeAdapterConfig<CartProduct, CartItemDto>
                .NewConfig()
                .Map(dest => dest.ProductId, src => src.ProductId)
                .Map(dest => dest.Quantity, src => src.Quantity)
                .Map(dest => dest.ProductName, src => src.Product.ProductName)
                .Map(dest => dest.Price, src => src.Product.Price);

            TypeAdapterConfig<Cart, CartResponseDto>
                .NewConfig()
                .Map(dest => dest.UserId, src => src.UserId)
                .Map(dest => dest.Items, src => src.CartProducts);
        }
    }
}
