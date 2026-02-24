using ECommerceApp.Application.Interfaces.Rebositories.ICartRepository;
using ECommerceApp.Domain.Entities;
using ECommerceApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Infrastructure.Repositories.CartRepository
{
    public class CartRepository:ICartRepository
    {
        public ApplicationDbContext DbContext { get; }

        public CartRepository(ApplicationDbContext applicationDbContext)
        {
            DbContext = applicationDbContext;
        }


        public Cart GetCartByCustomerId(int customerId)
        {
            return DbContext.Carts
                 .Include(c => c.CartProducts)
                     .ThenInclude(cp => cp.Product)
                 .FirstOrDefault(c => c.UserId == customerId);
        }

        public void Add(Cart entity)
        {
            if (entity != null)
            {
                DbContext.Carts.Add(entity);
            }
        }

        public void Delete(Cart entity)
        {
            DbContext.Carts.Remove(entity);
        }




        public void SaveCahange()
        {
            DbContext.SaveChanges();
        }

        public void Update(Cart entity)
        {
            if (entity != null)
            {
                DbContext.Update(entity);

            }
        }

        public IQueryable<Cart> GetAll()
        {
            return DbContext.Carts.Include(e=>e.CartProducts);
        }

        public void SaveChange()
        {
            DbContext.SaveChanges();
        }
    }
}
