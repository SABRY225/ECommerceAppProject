using ECommerceApp.Application.DTOs.Customer;
using ECommerceApp.Application.Interfaces.Services;
using ECommerceApp.Application.Services;
using ECommerceApp.Domain.Entities;
using ECommerceApp.Infrastructure.Data;
using ECommerceApp.Infrastructure.Repositories;
using ECommerceApp.Presentation.Auth;
using Microsoft.Web.WebView2.WinForms;

namespace ECommerceApp.Presentation.Client
{
    public partial class LoginForm : Form
    {
        private WebView2 webView;
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public ICustomerUserService UserService { get; set; }
        private IProductService _productService;
        private IOrderService _orderService;
        private ICartService _cartService;
        private readonly ApplicationDbContext dbContext = new ApplicationDbContext();

        public LoginForm()
        {
            InitializeComponent();
            this.Text = "E-Comm Suite - Login";
            this.WindowState = FormWindowState.Maximized;
            dbContext = new ApplicationDbContext();
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
<hr>
<button class='btn btn-outline-secondary w-100' onclick='SginAdmin()'>
    Sign Admin
</button>

    <hr>
    <p class='text-center text-muted'>New to E-Comm Suite?</p>
    <button class='btn btn-outline-secondary w-100' onclick='createAccount()'>
        Create a New Customer Account
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
function createAccount(){
    var data = { action: 'createAccount' };
    window.chrome.webview.postMessage(data);
}
function SginAdmin(){
    var data = { action: 'SginAdmin' };
    window.chrome.webview.postMessage(data);
}
// Toggle buttons
document.querySelectorAll('.toggle-btn').forEach(btn => {
    btn.addEventListener('click', function(){
        document.querySelectorAll('.toggle-btn').forEach(b=>b.classList.remove('active'));
        this.classList.add('active');
    });
});

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
                            var user = UserService.Login(data);

                            if (user != null && user.Role !="1")
                            {
                                UserSession.CustomerId = user.Id; 
                                UserSession.CustomerName = user.FirstName;

                                var successMessage = new { type = "success", message = "Login Success!" };
                                webView.CoreWebView2.PostWebMessageAsJson(System.Text.Json.JsonSerializer.Serialize(successMessage));
                                var context = new ApplicationDbContext();
                                var productRepo = new GenericRebository<Product>(context);
                                var orderRepo = new GenericRebository<Order>(context);
                                var cartRepo = new GenericRebository<Cart>(context);
                                _productService = new ProductService(productRepo);
                                _orderService = new OrderService(orderRepo, cartRepo, productRepo);
                                _cartService = new CartService(cartRepo, productRepo);

                                var clientForm = new ProductsForm(_productService, _orderService, _cartService);
                                clientForm.Show();
                                this.Hide();
                            }
                        }
                        catch (Exception ex)
                        {
                            var errorMessage = new { type = "error", message = ex.Message };
                            webView.CoreWebView2.PostWebMessageAsJson(System.Text.Json.JsonSerializer.Serialize(errorMessage));
                        }
                        break;

                    case "createAccount":
                        var registerForm = new RegisterForm(); 
                        registerForm.Show();
                        this.Hide();
                        break;
                    case "SginAdmin":
                        var signsAdminForm = new SignAdmin();
                        signsAdminForm.Show();
                        this.Hide();
                        break;
                }
            }
        }
    }
}