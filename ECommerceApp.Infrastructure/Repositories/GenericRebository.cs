using ECommerceApp.Application.Interfaces.Rebositories;
using ECommerceApp.Domain;
using ECommerceApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ECommerceApp.Infrastructure.Repositories
{
   public  class GenericRebository<T>(ApplicationDbContext dbContext) : IGenericRebository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _context = dbContext;

        public async Task Add(T entity)
        {
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
            
        }

        public async Task Delete(T entity)
        {
            if (entity != null)
            {
                _context.Remove(entity);
                await _context.SaveChangesAsync();
            }

        }

        public IQueryable<T> FindAsync(Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>().Where(expression);
        }

        public IQueryable<T> GetAll()
        {
            var result = _context.Set<T>();
            return result;
        }

        public async Task<T?> GetByIdAsync(
             int id,
             List<Expression<Func<T, object>>>? includes = null
            )
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return await query.FirstOrDefaultAsync(e =>e.Id == id);
        }

        public async Task Update(T entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();

         }
    }
}
