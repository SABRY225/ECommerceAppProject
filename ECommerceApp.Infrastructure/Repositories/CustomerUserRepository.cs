using ECommerceApp.Domain.Entities;
using ECommerceApp.Infrastructure.Data;
using ECommerceApp.Application.Interfaces.Rebositories.ICustomerUserRepository;
using ECommerceApp.Infrastructure.Enums;


namespace ECommerceApp.Infrastructure.Repositories
{
    public class CustomerUserRepository : ICustomerUserRepository
    {
        public ApplicationDbContext DbContext { get; }
        public CustomerUserRepository(ApplicationDbContext applicationDbContext)
        {
            DbContext = applicationDbContext;
        }


        public User GetUserByCredentials(string Email, string Password)
        {
            var useraccount = DbContext.Users.FirstOrDefault(u => u.Email == Email && u.Password == Password);
            if (useraccount is not null)
            {
                return useraccount;
            }
            return null;
        }

        public void RegisterCustomerAccount(User user)
        {
            //bool emailExists = DbContext.Users
            //    .Any(u => u.Email == user.Email && !u.IsDeleted);

            //if (emailExists)
            //    throw new Exception("Email already exists.");

            user.Role = "2";
            user.Phone = "0102356558";
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            user.IsDeleted = false;
            DbContext.Users.Add(user);
            //DbContext.SaveChanges();
        }

        public void SaveChanges()
        {
            DbContext.SaveChanges();
        }

        public User GetUserEmail(string email)
        {
            var user = DbContext.Users.FirstOrDefault(e => e.Email == email);
            return user;
        }
    }
}
