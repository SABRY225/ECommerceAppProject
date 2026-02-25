using ECommerceApp.Application.DTOs.Cart;

namespace ECommerceApp.Application.Interfaces.Services
{
    public interface ICartService
    {
        public Task<bool> AddToCart(AddProductToCartDto addProductToCartDto);
        public Task<bool> RemoveItem(int userId ,int id);
        public Task<bool> UpdateItemQuantity(int userId, int productId, int quantity);
        public Task<CartDto> GetCustomerCart(int UserId);
    }
}
