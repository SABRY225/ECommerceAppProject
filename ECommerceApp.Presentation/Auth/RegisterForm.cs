using ECommerceApp.Application.DTOs.Customer;
using ECommerceApp.Application.Interfaces.Rebositories;
using ECommerceApp.Application.Interfaces.Rebositories.ICustomerUserRepository;
using ECommerceApp.Application.Interfaces.Services;
using ECommerceApp.Application.Services;
using ECommerceApp.Domain.Entities;
using ECommerceApp.Infrastructure.Data;
using ECommerceApp.Infrastructure.Repositories;
using ECommerceApp.Presentation.Auth;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Text.Json;
using System.Windows.Forms;


namespace ECommerceApp.Presentation.Client
{
    public partial class RegisterForm : Form
    {
        private WebView2 webView;
        private readonly ApplicationDbContext dbContext = new ApplicationDbContext();
        private readonly ICustomerUserRepository userRepository;
        private readonly ICustomerUserService userService;
        public RegisterForm()
        {
            InitializeComponent();

            dbContext = new ApplicationDbContext();
            IGenericRebository<User> _genericRebository = new GenericRebository<User>(dbContext);
            userRepository = new CustomerUserRepository(dbContext);
            userService = new CustomerUserService(userRepository, _genericRebository);

            this.Text = "E-Comm Suite - Register Client";
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
            string html = @"<!doctype html>
<html lang=""en"">
<head>
<meta charset=""UTF-8"">
<meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
<title>Create Account</title>

<link href=""https://fonts.googleapis.com/css2?family=Poppins:wght@400;500;600&display=swap"" rel=""stylesheet"">

<style>
* {
    box-sizing: border-box;
}

body {
    font-family: 'Poppins', sans-serif;
    background: linear-gradient(135deg, #1a374d, #406882);
    display: flex;
    justify-content: center;
    align-items: center;
    height: 100vh;
    margin: 0;
}

.card {
    background: #ffffff;
    padding: 35px;
    border-radius: 18px;
    box-shadow: 0 15px 40px rgba(0,0,0,0.15);
    width: 480px;
    animation: fadeIn 0.6s ease-in-out;
}

@keyframes fadeIn {
    from { opacity:0; transform:translateY(15px); }
    to { opacity:1; transform:translateY(0); }
}

h2 {
    text-align: center;
    margin-bottom: 25px;
    color: #1a374d;
    font-weight: 600;
}

.input-group {
    margin-bottom: 18px;
}

label {
    display: block;
    font-size: 0.85rem;
    margin-bottom: 6px;
    font-weight: 500;
    color: #444;
}

input {
    width: 100%;
    padding: 12px 14px;
    border: 1px solid #ddd;
    border-radius: 10px;
    background: #f8fafc;
    font-size: 14px;
    transition: 0.3s ease;
}

input:focus {
    outline: none;
    border-color: #406882;
    background: #fff;
    box-shadow: 0 0 0 3px rgba(64,104,130,0.15);
}

.row {
    display: flex;
    gap: 12px;
}

button {
    width: 100%;
    padding: 13px;
    background: linear-gradient(135deg, #1a374d, #406882);
    color: white;
    border: none;
    border-radius: 12px;
    cursor: pointer;
    font-size: 15px;
    font-weight: 500;
    margin-top: 10px;
    transition: 0.3s ease;
}

button:hover {
    transform: translateY(-2px);
    box-shadow: 0 8px 20px rgba(0,0,0,0.2);
}

.secondary-btn {
    background: #f1f5f9;
    color: #1a374d;
    margin-top: 10px;
}

.secondary-btn:hover {
    background: #e2e8f0;
}

hr {
    margin: 20px 0;
    border: none;
    border-top: 1px solid #eee;
}

/* Responsive */
@media (max-width: 500px) {
    .card {
        width: 95%;
        padding: 25px;
    }
    .row {
        flex-direction: column;
    }
}
</style>
</head>

<body>

<div class='card'>
<h2>Create Account</h2>

<div class='row'>
  <div class='input-group'>
    <label>First Name</label>
    <input type='text' id='FirstName' placeholder=""Enter first name""/>
  </div>

  <div class='input-group'>
    <label>Last Name</label>
    <input type='text' id='LastName' placeholder=""Enter last name""/>
  </div>
</div>

<div class='input-group'>
  <label>Email</label>
  <input type='email' id='Email' placeholder=""example@email.com""/>
</div>

<div class='input-group'>
  <label>Phone Number</label>
  <input type='text' id='PhoneNumber' placeholder=""+20 10 0000 0000""/>
</div>

<div class='input-group'>
  <label>Password</label>
  <input type='password' id='Password' placeholder=""Enter password""/>
</div>

<div class='input-group'>
  <label>Address</label>
  <input type='text' id='Address' placeholder=""Your address""/>
</div>

<button onclick='register()'>Register Account →</button>

<hr>

<button class='secondary-btn' onclick='Back()'>
Back to Sign In
</button>

</div>

<script>
function register(){
    const data = {
        action: 'register',
        FirstName: document.getElementById('FirstName').value,
        LastName: document.getElementById('LastName').value,
        Email: document.getElementById('Email').value,
        PhoneNumber: document.getElementById('PhoneNumber').value,
        Password: document.getElementById('Password').value,
        Address: document.getElementById('Address').value
    };

    window.chrome.webview.postMessage(data);
}

function Back(){
    var data = { action: 'back' };
    window.chrome.webview.postMessage(data);
}
</script>

</body>
</html>
";

            webView.NavigateToString(html);
        }

        private void CoreWebView2_WebMessageReceived(object sender,
            Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            
            string json = e.WebMessageAsJson;
            using var doc = System.Text.Json.JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.TryGetProperty("action", out var action))
            {
                switch (action.GetString())
                {
                    case "register":
                        var data = System.Text.Json.JsonSerializer.Deserialize<RegitserCustomerUserDto>(json);
                        if (data == null) return;

                        try
                        {
                            userService.RegisterAccount(data);
                            var loginForm = new LoginForm();
                            loginForm.Show();
                            MessageBox.Show("Registration Successful ✅");

                        }
                        catch (Exception ex)
                        {
                            var errorMessage = new { type = "error", message = ex.Message };
                            webView.CoreWebView2.PostWebMessageAsJson(System.Text.Json.JsonSerializer.Serialize(errorMessage));
                        }
                        break;

                    case "back":
                        var loginFormBack = new LoginForm();
                        loginFormBack.Show();
                        this.Hide();
                        break;
                }
            }
            
        }
}
}
