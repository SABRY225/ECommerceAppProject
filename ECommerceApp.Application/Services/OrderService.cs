using ECommerceApp.Application.DTOs.Order;
using ECommerceApp.Application.Interfaces.Rebositories;
using ECommerceApp.Application.Interfaces.Services;
using ECommerceApp.Domain.Entities;
using ECommerceApp.Infrastructure.Enums;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Application.Services
{
    public class OrderService(IGenericRebository<Order> orderRebository,
        IGenericRebository<Cart> cartRebository,
        IGenericRebository<Product> productRebository) : IOrderService
    {
        private readonly IGenericRebository<Order> _orderService = orderRebository;
        private readonly IGenericRebository<Cart> _cartService = cartRebository;
        private readonly IGenericRebository<Product> _productService = productRebository;


        public async Task<string> CreateOrder(int cartId)
        {
            var cart = await _cartService
                .FindAsync(c => c.Id == cartId && !c.IsDeleted)
                .Include(c => c.User)
                .Include(c => c.CartProducts)
                .ThenInclude(cp => cp.Product)
                .FirstOrDefaultAsync();

            if (cart == null)
                return "cart not found";

            if (cart.CartProducts.Count == 0)
                return "cart is empty";

            foreach (var item in cart.CartProducts)
            {
                if (item.Product.StockQuantity < item.Quantity)
                    return $"quantity in stock less than your order in {item.Product.ProductName}";
            }

            var order = new Order
            {
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false,
                OrderDate = DateTime.UtcNow,
                State = OrderState.Pending.ToString(),
                TotalAmount = cart.CartProducts.Sum(cp => cp.Quantity * cp.Product.Price),
                UserId = cart.UserId,
                OrderProducts = []
            };

            foreach (var item in cart.CartProducts)
            {
                order.OrderProducts.Add(new OrderProduct
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    PriceAtPurchase = item.Product.Price
                });

                item.Product.StockQuantity -= item.Quantity;
            }

            await _orderService.Add(order);

            var result = await _orderService.SaveAsync();

            if (!result)
                return "failed";

            cart.CartProducts.Clear();
            await _cartService.SaveAsync();

            return "done";
        }

        public async Task<List<GetOrderDto>> GetAllOrders()
        {
            var orders = await _orderService.GetAll()
                .Include(o=>o.OrderProducts)
                .ThenInclude(op=>op.Product)
                .Include(o=>o.User)
                .ToListAsync();
            return orders.Adapt<List<GetOrderDto>>();
        }

        public async Task<List<GetOrderDto>> GetCustomerOrders(int cuatomerId)
        {
            var orders = await _orderService.FindAsync(o=>o.User.Id == cuatomerId )
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .Include(o => o.User)
                .ToListAsync();
            return orders.Adapt<List<GetOrderDto>>();
        }

        public async Task<GetOrderDto> GetOrder(int id)
        {
            var product = await _orderService.FindAsync(o=>o.Id == id).Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .Include(o=>o.User)
                .FirstOrDefaultAsync();
            return product.Adapt<GetOrderDto>();
        }

        public async Task<bool> UpdateOrderStatus(UpdateOrderDto updateOrderDto)
        {
            var product = await _orderService.FindAsync(o => o.Id == updateOrderDto.Id).Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product).FirstOrDefaultAsync();
            if(product == null) return false;
            product.State = updateOrderDto.State;
            return await _orderService.SaveAsync();
        }
    }
}
