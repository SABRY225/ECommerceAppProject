using System;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;

namespace ECommerceApp.Presentation.Client
{
    public partial class LoginForm : Form
    {
        private WebView2 webView;

        public LoginForm()
        {
            InitializeComponent();

            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new System.Drawing.Size(1200, 800);

            InitializeWebView();
        }
        private async void InitializeWebView()
        {
            webView = new WebView2();
            webView.Dock = DockStyle.Fill;
            this.Controls.Add(webView);

            await webView.EnsureCoreWebView2Async(null);

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

    <div class='btn-group w-100 mb-3'>
        <button class='btn btn-light toggle-btn active'>Customer</button>
        <button class='btn btn-light toggle-btn'>Administrator</button>
    </div>

    <div class='mb-3'>
        <label class='form-label'>Email or Username</label>
        <input type='text' class='form-control' placeholder='e.g. johndoe@company.com'>
    </div>

    <div class='mb-3'>
        <label class='form-label'>Password</label>
        <input type='password' class='form-control' placeholder='********'>
    </div>

    <div class='form-check mb-3'>
        <input class='form-check-input' type='checkbox'>
        <label class='form-check-label'>Remember this device</label>
    </div>

    <button class='btn btn-primary w-100 mb-3' onclick='login()'>
        Sign In
    </button>

    <hr>
    <p class='text-center text-muted'>New to E-Comm Suite?</p>
    <button class='btn btn-outline-secondary w-100'>
        Create a New Customer Account
    </button>
</div>

<script>
function login(){
    alert('Login Clicked');
}
</script>

</body>
</html>";

            webView.NavigateToString(html);
        }
    }
}