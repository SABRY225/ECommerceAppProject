using ECommerceApp.Domain.Entities;
using ECommerceApp.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Infrastructure.Repositories
{
    public class CustomerUserRepository
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
            if (user != null)
            {
                DbContext.Users.Add(user);
            }
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
