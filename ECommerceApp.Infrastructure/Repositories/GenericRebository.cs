using ECommerceApp.Application.Interfaces.Rebositories;
using ECommerceApp.Domain;
using ECommerceApp.Infrastructure.Data;

namespace ECommerceApp.Infrastructure.Repositories
{
   public  class GenericRebository<T> : IGenericRebository<T> where T : BaseEntity
    {
        private ApplicationDbContext _context ;

       public GenericRebository(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        public void Add(T entity)
        {
            _context.Add(entity);
           _context.SaveChanges();
            
        }

        public void Delete(T entity)
        {
            if (entity != null)
            {
                _context.Remove(entity);
                _context.SaveChanges();
            }

        }

        public IQueryable<T> GetAll()
        {
            var result = _context.Set<T>();
            return result;
        }

        public void Update(T entity)
        {
            _context.Update(entity);
            _context.SaveChanges();

         }
    }
}
