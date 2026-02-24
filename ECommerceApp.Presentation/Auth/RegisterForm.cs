using System;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;


namespace ECommerceApp.Presentation.Client
{
    public partial class RegisterForm : Form
    {
        private WebView2 webView;

        public RegisterForm()
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
<!doctype html>
<html>
  <head>
    <style>
      body {
        font-family: sans-serif;
        background-color: #f4f7f9;
        display: flex;
        justify-content: center;
        padding: 20px;
      }
      .card {
        background: white;
        padding: 30px;
        border-radius: 12px;
        box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
        width: 400px;
      }
      h2 {
        color: #1a374d;
        text-align: center;
        margin-bottom: 5px;
      }
      p {
        text-align: center;
        color: #666;
        font-size: 0.9em;
      }
      .input-group {
        margin-bottom: 15px;
      }
      label {
        display: block;
        margin-bottom: 5px;
        color: #1a374d;
        font-weight: bold;
        font-size: 0.9em;
      }
      input {
        width: 100%;
        padding: 10px;
        border: 1px solid #ddd;
        border-radius: 6px;
        box-sizing: border-box;
        background: #f9f9f9;
      }
      .row {
        display: flex;
        gap: 10px;
      }
      .btn-register {
        width: 100%;
        padding: 12px;
        background: #1a374d;
        color: white;
        border: none;
        border-radius: 6px;
        cursor: pointer;
        font-weight: bold;
        margin-top: 10px;
      }
      .btn-login {
        width: 100%;
        padding: 10px;
        background: white;
        color: #1a374d;
        border: 1px solid #ddd;
        border-radius: 6px;
        margin-top: 10px;
        cursor: pointer;
      }
    </style>
  </head>
  <body>
    <div class=""card"">
      <h2>Create Account</h2>
      <p>Join our professional network.</p>

      <div class=""input-group"">
        <label>Full Name</label>
        <input type=""text"" id=""txtName"" placeholder=""John Doe"" />
      </div>
      <div class=""input-group"">
        <label>Email Address</label>
        <input type=""email"" id=""txtEmail"" placeholder=""example@domain.com"" />
      </div>
      <div class=""input-group"">
        <label>Password</label>
        <input type=""password"" id=""txtPass"" />
      </div>
      <div class=""input-group"">
        <label>Confirm</label>
        <input type=""password"" id=""txtConfirm"" />
      </div>

      <button class=""btn-register"" onclick=""register()"">
        Register Account →
      </button>
      <button
        class=""btn-login""
        onclick=""window.chrome.webview.postMessage('go_to_login')""
      >
        Back to Login
      </button>
    </div>

    <script>
      function register() {
        const data = {
          name: document.getElementById(""txtName"").value,
          email: document.getElementById(""txtEmail"").value,
        };
        
        window.chrome.webview.postMessage(data);
      }
    </script>
  </body>
</html>
";

            webView.NavigateToString(html);
        }
    }
}
