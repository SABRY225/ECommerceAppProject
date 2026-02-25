using ECommerceApp.Application.DTOs.Cart;
using ECommerceApp.Application.Interfaces.Rebositories.ICartRepository;
using ECommerceApp.Application.Interfaces.Services;
using ECommerceApp.Application.Services.CartService;
using ECommerceApp.Infrastructure.Data;
using ECommerceApp.Infrastructure.Repositories.CartRepository;

namespace resention
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ApplicationDbContext dbcontext = new ApplicationDbContext();
            ICartRepository cartRepository = new CartRepository(dbcontext);
            ICartService cartService = new CartService(cartRepository);

            CartItemRequest dto = new CartItemRequest { ProductId = 24, UserId = 6, Quantity = 5 };
            cartService.AddProductToCart(dto);

        }
    }
}
