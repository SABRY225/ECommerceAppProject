using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ECommerceApp.Application.Interfaces.Rebositories
{
    public interface IGenericRebository<T>
    {
        public IQueryable<T> GetAll();
        public IQueryable<T> FindAsync(Expression<Func<T,bool>> expression);
        public Task<T?> GetByIdAsync(int id,List<Expression<Func<T, object>>>? includes = null);
        public Task Add(T entity);
        public Task Update(T entity);
        public Task Delete(T entity);
        Task<bool> SaveAsync();
    }
}
