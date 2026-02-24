using ECommerceApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Interfaces.Rebositories.ICustomerUserRepository
{
    public interface ICustomerUserRepository
    {
        public User GetUserByCredentials(string Email, string Password);
        public void RegisterCustomerAccount(User user);
        public User GetUserEmail(string email);
        public void SaveChanges();
    }
}
