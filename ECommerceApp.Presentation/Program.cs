using ECommerceApp.Application.Interfaces.Rebositories;
using ECommerceApp.Application.Interfaces.Rebositories.ICustomerUserRepository;
using ECommerceApp.Application.Interfaces.Services;
using ECommerceApp.Application.Mapper;
using ECommerceApp.Application.Services;
using ECommerceApp.Domain.Entities;
using ECommerceApp.Infrastructure.Data;
using ECommerceApp.Infrastructure.Repositories;
using ECommerceApp.Presentation.Client;

namespace ECommerceApp.Presentation
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {

            MapsterConfig.Config();
            
            ApplicationConfiguration.Initialize();
            var dbContext = new ApplicationDbContext();
            // create Repository
            ICustomerUserRepository userRepository = new CustomerUserRepository(dbContext);
            IGenericRebository<User> _genericRebository = new GenericRebository<User> (dbContext);

            //  create  Service
            ICustomerUserService userService = new CustomerUserService(userRepository, _genericRebository);

            // create form
            var loginForm = new LoginForm();

            // (Property Injection)
            loginForm.UserService = userService;

            System.Windows.Forms.Application.Run(loginForm);
        }
    }
}

