using ECommerceApp.Application.DTOs.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<bool> CreateOrder(int cartId);
        Task<bool> UpdateOrderStatus(UpdateOrderDto updateOrderDto);
        Task<GetOrderDto> GetOrder(int id);
        Task<List<GetOrderDto>> GetCustomerOrders(int cuatomerId);
        Task<List<GetOrderDto>> GetAllOrders();
    }
}
