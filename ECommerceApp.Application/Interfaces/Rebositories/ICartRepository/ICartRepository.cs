using ECommerceApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Interfaces.Rebositories.ICartRepository
{
    public interface ICartRepository:IGenericRebository<Cart>
    {
        Cart GetCartByCustomerId(int customerId);
    }
}
