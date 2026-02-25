using ECommerceApp.Application.DTOs.Order;
using ECommerceApp.Application.Interfaces.Rebositories;
using ECommerceApp.Application.Interfaces.Services;
using ECommerceApp.Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Application.Services
{
    public class OrderService(IGenericRebository<Order> orderRebository) : IOrderService
    {
        private readonly IGenericRebository<Order> _orderService = orderRebository;
        public Task<bool> CreateOrder(int cartId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<GetOrderDto>> GetAllOrders()
        {
            var orders = await _orderService.GetAll()
                .Include(o=>o.OrderProducts)
                .ThenInclude(op=>op.Product)
                .ToListAsync();
            return orders.Adapt<List<GetOrderDto>>();
        }

        public async Task<List<GetOrderDto>> GetCustomerOrders(int cuatomerId)
        {
            var orders = await _orderService.FindAsync(o=>o.User.Id == cuatomerId )
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .ToListAsync();
            return orders.Adapt<List<GetOrderDto>>();
        }

        public async Task<GetOrderDto> GetOrder(int id)
        {
            var product = await _orderService.FindAsync(o=>o.Id == id).Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product).FirstOrDefaultAsync();
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
