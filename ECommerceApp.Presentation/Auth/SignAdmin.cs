using ECommerceApp.Application.DTOs.Customer;
using ECommerceApp.Application.Interfaces.Rebositories;
using ECommerceApp.Application.Interfaces.Rebositories.ICustomerUserRepository;
using ECommerceApp.Application.Interfaces.Services;
using ECommerceApp.Application.Services;
using ECommerceApp.Domain.Entities;
using ECommerceApp.Infrastructure.Data;
using ECommerceApp.Infrastructure.Enums;
using ECommerceApp.Infrastructure.Repositories;
using ECommerceApp.Presentation.Admin;
using ECommerceApp.Presentation.Client;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Windows.Forms;

namespace ECommerceApp.Presentation.Auth
{
    public partial class SignAdmin : Form
    {
        private WebView2 webView;
        private readonly ApplicationDbContext dbContext = new ApplicationDbContext();
        private readonly ICustomerUserRepository userRepository;
        private readonly ICustomerUserService userService;
        private ICategoryService _categoryService;
        private IProductService _productService;
        private IOrderService _orderService;
        private ICartService _cartService;

        public SignAdmin()
        {
            InitializeComponent();
            dbContext = new ApplicationDbContext();
            IGenericRebository<User> _genericRebository = new GenericRebository<User>(dbContext);
            userRepository = new CustomerUserRepository(dbContext);
            userService = new CustomerUserService(userRepository, _genericRebository);

            this.Text = "E-Comm Suite - Admin Login";
            this.WindowState = FormWindowState.Maximized;
            InitializeWebView();
        }


        private async void InitializeWebView()
        {
            webView = new WebView2();
            webView.Dock = DockStyle.Fill;
            this.Controls.Add(webView);
            await webView.EnsureCoreWebView2Async(null);
            webView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;

            string html = @"
<!DOCTYPE html>
<html>
<head>
<meta charset='UTF-8'>
<meta name='viewport' content='width=device-width, initial-scale=1'>
<link href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css' rel='stylesheet'>
<style>
body{
    background:#f4f6f9;
    display:flex;
    justify-content:center;
    align-items:center;
    height:100vh;
}
.card{
    width:400px;
    border:none;
    border-radius:15px;
    box-shadow:0 10px 25px rgba(0,0,0,0.1);
}
.btn-primary{
    background:#1f3b57;
    border:none;
}
.btn-primary:hover{
    background:#162c40;
}
.toggle-btn{
    width:50%;
}
</style>
</head>

<body>

<div class='card p-4'>
    <h3 class='text-center fw-bold'>Welcome Back</h3>
    <p class='text-center text-muted'>Please enter your details to sign in</p>

    <div class='mb-3'>
        <label class='form-label'>Email</label>
        <input type='email' class='form-control' id='email' placeholder='e.g. johndoe@company.com'>
    </div>

    <div class='mb-3'>
        <label class='form-label'>Password</label>
        <input type='password' id='password' class='form-control' placeholder='********'>
    </div>

    <button class='btn btn-primary w-100 mb-3' onclick='login()'>
        Sign In
    </button>

</div>

<div id=""login-message"" class=""text-center mb-3"" style=""height:20px;""></div>

<script>
function login(){
    var email = document.getElementById('email').value;
    var password = document.getElementById('password').value;

    var data = {
        action: 'login',
        Email: email,
        Password: password
    };

    window.chrome.webview.postMessage(data);
}


// استقبال الرسائل من C#
window.chrome.webview.addEventListener('message', function(event){
    var msg = event.data;
    var msgDiv = document.getElementById('login-message');

    if(msg.type === ""success""){
        msgDiv.style.color = ""green"";
        msgDiv.innerText = msg.message;
    } else if(msg.type === ""error""){
        msgDiv.style.color = ""red"";
        msgDiv.innerText = msg.message;
    }
});
</script>

</body>
</html>";

            webView.NavigateToString(html);
        }

        private void CoreWebView2_WebMessageReceived(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            var json = e.WebMessageAsJson;
            using var doc = System.Text.Json.JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.TryGetProperty("action", out var action))
            {
                switch (action.GetString())
                {
                    case "login":
                        var data = System.Text.Json.JsonSerializer.Deserialize<LoginUserDto>(json);
                        if (data == null) return;

                        try
                        {
                            userService.Login(data);

                            var successMessage = new { type = "success", message = "Login Success!" };
                            webView.CoreWebView2.PostWebMessageAsJson(System.Text.Json.JsonSerializer.Serialize(successMessage));
                            var context = new ApplicationDbContext();
                            var categoryRepo = new GenericRebository<Category>(context);
                            var productRepo = new GenericRebository<Product>(context);
                            var orderRepo = new GenericRebository<Order>(context);
                            var cartRepo = new GenericRebository<Cart>(context);
                            _categoryService = new CategoryService(categoryRepo);
                            _productService = new ProductService(productRepo);
                            _orderService = new OrderService(orderRepo, cartRepo, productRepo);

                            var adminForm = new DashboardForm(_categoryService, _productService, _orderService);
                            adminForm.Show();

                            this.Hide();
                        }
                        catch (Exception ex)
                        {
                            var errorMessage = new { type = "error", message = ex.Message };
                            webView.CoreWebView2.PostWebMessageAsJson(System.Text.Json.JsonSerializer.Serialize(errorMessage));
                        }
                        break;
                }
            }
        }
    }
}
