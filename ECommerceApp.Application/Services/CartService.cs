using ECommerceApp.Application.DTOs.Cart;
using ECommerceApp.Application.Interfaces.Rebositories;
using ECommerceApp.Application.Interfaces.Services;
using ECommerceApp.Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Services
{
    public class CartService(IGenericRebository<Cart> genericRebository, IGenericRebository<Product> productRebository) : ICartService
    {
        private readonly IGenericRebository<Cart> _repo = genericRebository;
        private readonly IGenericRebository<Product> _productRepo = productRebository;

        public async Task<bool> AddToCart(AddProductToCartDto dto)
        {
            var cart = await _repo.FindAsync(c => c.UserId ==dto.CustomerId && c.IsDeleted == false).Include(c => c.CartProducts).FirstOrDefaultAsync();
            var product = await _productRepo.GetByIdAsync(dto.ProductId);
            if (product == null) return false;
            if (cart != null)
            {
                var existingProduct = cart.CartProducts
                .FirstOrDefault(cp => cp.ProductId == dto.ProductId);

                if (existingProduct != null)
                {
                    existingProduct.Quantity++;
                }
                else
                {
                    cart.CartProducts.Add(new CartProduct
                    {
                        Product = product,
                        Quantity = 1,
                        Date = DateTime.UtcNow
                    });
                }
                return await _repo.SaveAsync();
            }
            else
            {
                var newCart = new Cart
                {
                    UserId = dto.CustomerId,
                    IsDeleted = false,
                    CartProducts =
                    [
                            new CartProduct
                            {
                                Product = product,
                                Quantity = 1,
                                Date = DateTime.UtcNow
                            }
                    ]
                };

                await _repo.Add(newCart);
                return await _repo.SaveAsync();
            }
        }

        public async Task<CartDto> GetCustomerCart(int id)
        {
            var cart = await _repo.FindAsync(c => c.UserId == id && c.IsDeleted == false).Include(c => c.CartProducts).ThenInclude(cp=>cp.Product).FirstOrDefaultAsync();
            if (cart == null) return new();

            var cartDto = cart.Adapt<CartDto>();
            cartDto.TotalAmount = cartDto.CartProducts.Sum(cd => cd.ItemTotal);
            return cartDto;
        }

        public async Task<bool> RemoveItem(int userId ,int id)
        {
            var cart = await _repo.FindAsync(c => c.UserId == userId).Include(c => c.CartProducts).FirstOrDefaultAsync();
            var itemToRemove = cart?.CartProducts.FirstOrDefault(cp=>cp.ProductId == id);
            if (itemToRemove == null) return false;
            cart?.CartProducts.Remove(itemToRemove);
            return await _repo.SaveAsync();
        }

        public async Task<bool> UpdateItemQuantity(int userId ,int id ,int quantity)
        {
            var cart = await _repo.FindAsync(c => c.UserId == userId).Include(c => c.CartProducts).FirstOrDefaultAsync();
            var itemToUpdate = cart?.CartProducts.FirstOrDefault(cp => cp.ProductId == id);
            if (itemToUpdate == null) return false;
            itemToUpdate.Quantity = quantity;
            return await _repo.SaveAsync();
        }
    }
}
