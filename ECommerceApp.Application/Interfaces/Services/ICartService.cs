using ECommerceApp.Application.DTOs.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Interfaces.Services
{
    public interface ICartService
    {
        CartResponseDto GetCustomerCartByID(int userId);
        void AddProductToCart(CartItemRequest request);
        void UpdateCartItem(CartItemRequest request);
        void RemoveProductFromCart(int userId, int productId);
        IEnumerable<CartResponseDto> GetAllCarts();
    }
}
