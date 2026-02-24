using ECommerceApp.Application.Interfaces.Rebositories;
using ECommerceApp.Domain;
using ECommerceApp.Infrastructure.Data;

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

        public IQueryable<T> GetAll()
        {
            var result = _context.Set<T>();
            return result;
        }

        public async Task Update(T entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();

         }
    }
}
