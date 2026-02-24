using ECommerceApp.Application.DTOs.Cart;
using ECommerceApp.Application.Interfaces.Rebositories.ICartRepository;
using ECommerceApp.Application.Interfaces.Services;
using ECommerceApp.Application.Mapping.CartMapping;
using ECommerceApp.Domain.Entities;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Services.CartService
{
    public class CartService:ICartService
    {
        public ICartRepository CartRepository { get; }

        public CartService(ICartRepository cartRepository)
        {
            CartRepository = cartRepository;
        }


        public void AddProductToCart(CartItemRequest request)
        {
            var cart = CartRepository.GetCartByCustomerId(request.UserId);
            if (cart == null)
            {
                cart = new Cart { UserId = request.UserId, CartProducts = new List<CartProduct>() };
                CartRepository.Add(cart);
            }


            var existingItem = cart.CartProducts.FirstOrDefault(cp => cp.ProductId == request.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += request.Quantity;
                existingItem.Date = DateTime.Now;
            }
            else
            {
                var newProduct = request.Adapt<CartProduct>();
                cart.CartProducts.Add(newProduct);
            }

            CartRepository.SaveChange();
        }

        public CartResponseDto GetCustomerCartByID(int userId)
        {
            var Cart_CartProduct_Product = CartRepository.GetCartByCustomerId(userId);

            if (Cart_CartProduct_Product == null)
            {
                return new CartResponseDto
                {
                    UserId = userId,
                    Items = new List<CartItemDto>()
                };
            }
            CartMapping.RegisterMappings();
            return Cart_CartProduct_Product.Adapt<CartResponseDto>();
        }

        public void RemoveProductFromCart(int userId, int productId)
        {
            var Cart_CartProduct_Product = CartRepository.GetCartByCustomerId(userId);
            if (Cart_CartProduct_Product != null)
            {
                var CartProduct = Cart_CartProduct_Product.CartProducts.FirstOrDefault(cp => cp.ProductId == productId);
                if (CartProduct != null)
                {
                    Cart_CartProduct_Product.CartProducts.Remove(CartProduct);
                    CartRepository.SaveChange();
                }
            }

        }

        public void UpdateCartItem(CartItemRequest request)
        {
            var Cart_CartProduct_Product = CartRepository.GetCartByCustomerId(request.UserId);
            if (Cart_CartProduct_Product != null)
            {
                var CartProduct = Cart_CartProduct_Product.CartProducts.FirstOrDefault(cp => cp.ProductId == request.ProductId);
                if (request.Quantity <= 0)
                {
                    return;
                }

                // request.Adapt<CartProduct>(); new instance
                request.Adapt(CartProduct);
                CartRepository.SaveChange();
            }

        }

        public IEnumerable<CartResponseDto> GetAllCarts()
        {
            var cartsDb = CartRepository.GetAll();
            if (cartsDb != null)
            {
                CartMapping.RegisterMappings();
                return cartsDb.Adapt<IEnumerable<CartResponseDto>>();
            }
            return new List<CartResponseDto>();
        }
    }
}
