using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerceApp.Application.Interfaces.Rebositories
{
    public interface IGenericRebository<T>
    {
        public IQueryable<T> GetAll();
        public void Add(T entity);
        public void Update(T entity);
        public void Delete(T entity);
    }
}
