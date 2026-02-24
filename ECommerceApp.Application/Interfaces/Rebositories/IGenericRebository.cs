using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerceApp.Application.Interfaces.Rebositories
{
    public interface IGenericRebository<T>
    {
        public IQueryable<T> GetAll();
        public Task Add(T entity);
        public Task Update(T entity);
        public Task Delete(T entity);
    }
}
