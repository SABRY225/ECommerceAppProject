using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;
using System.Text.Json;

namespace ECommerceApp.Presentation.Admin
{
    public partial class CategoryForm : Form
    {
        private WebView2 webView;
        public CategoryForm()
        {
            InitializeComponent();
            this.Text = "E-Comm Suite - Category Managment";
            this.WindowState = FormWindowState.Maximized;
            InitializeWebView();
        }
        private async void InitializeWebView()
        {
            webView = new WebView2 { Dock = DockStyle.Fill };
            this.Controls.Add(webView);

            // تهيئة المحرك
            await webView.EnsureCoreWebView2Async(null);

            // كتابة كود الـ HTML و CSS مباشرة هنا
            string htmlContent = @"
            <!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css' rel='stylesheet'>
    <link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.0/font/bootstrap-icons.css'>
    <style>
        :root { --sidebar-bg: #1e3a58; --main-bg: #f4f7f9; --text-light: #a5b4c1; }
        body { background-color: var(--main-bg); font-family: 'Segoe UI', sans-serif; margin: 0; display: flex; height: 100vh; overflow: hidden; }
        
        /* Sidebar */
        .sidebar { width: 260px; background: var(--sidebar-bg); color: white; display: flex; flex-direction: column; }
        .sidebar-header { padding: 30px 20px; border-bottom: 1px solid rgba(255,255,255,0.1); }
        .nav-link { color: var(--text-light); padding: 15px 25px; text-decoration: none; display: flex; align-items: center; gap: 15px; transition: 0.3s; }
        .nav-link:hover, .nav-link.active { background: rgba(255,255,255,0.1); color: white; }
        .user-profile { margin-top: auto; padding: 20px; background: rgba(0,0,0,0.2); display: flex; align-items: center; gap: 12px; }

        /* Main */
        .main { flex: 1; padding: 40px; overflow-y: auto; }
        .header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 30px; }
        .btn-add { background: var(--primary); color: white; border: none; padding: 10px 25px; border-radius: 6px; cursor: pointer; font-weight: bold; }
        
        /* Cards */
        .stats { display: grid; grid-template-columns: repeat(4, 1fr); gap: 20px; margin-bottom: 30px; }
        .card { background: white; padding: 20px; border-radius: 12px; box-shadow: 0 1px 3px rgba(0,0,0,0.1); }
        .card small { color: #64748b; font-size: 12px; text-transform: uppercase; }
        .card h2 { margin: 10px 0 0; color: #1e293b; }

        /* Table */
        table { width: 100%; border-collapse: collapse; background: white; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 6px -1px rgba(0,0,0,0.1); }
        th { text-align: left; background: #f1f5f9; padding: 15px; color: #475569; font-size: 13px; }
        td { padding: 15px; border-bottom: 1px solid #f1f5f9; color: #334155; }
        .badge { background: #f1f5f9; padding: 5px 12px; border-radius: 20px; font-weight: bold; font-size: 12px; }
        .actions button { background: none; border: none; cursor: pointer; color: #64748b; margin-right: 10px; font-size: 18px; }
        .actions button:hover { color: #ef4444; }
    </style>
</head>
<body>

    <div class='sidebar'>
        <div class='sidebar-header d-flex align-items-center gap-2'>
            <div class='bg-light text-dark p-2 rounded'><i class='bi bi-shop'></i></div>
            <div>
                <div class='fw-bold' style='font-size: 0.9rem;'>ADMIN PANEL</div>
                <div style='font-size: 0.7rem; color: var(--text-light);'>E-Commerce System</div>
            </div>
        </div>
        <nav class='mt-3'>
            <a href='#' class='nav-link' onclick='navigate(""dashboard"")'><i class='bi bi-grid'></i> Dashboard</a>
            <a href='#' class='nav-link active' onclick='navigate(""categories"")'><i class='bi bi-tags'></i> Categories</a>
            <a href='#' class='nav-link' onclick='navigate(""products"")'><i class='bi bi-box-seam'></i> Products</a>
            <a href='#' class='nav-link' onclick='navigate(""orders"")'><i class='bi bi-cart'></i> Orders</a>
            <a href='#' class='nav-link' onclick='navigate(""users"")'><i class='bi bi-people'></i> Users</a>
            <a href='#' class='nav-link' onclick='navigate(""admin"")'><i class='bi bi-person-gear'></i> Admin Management</a>
        </nav>
<div class='user-profile d-flex align-items-center justify-content-center'>
    <button class='btn btn-link text-danger p-0' title='تسجيل الخروج' style='text-decoration: none;'>
        <i class='bi bi-box-arrow-right' style='font-size: 1.1rem; cursor: pointer;'></i>
        LogOut
    </button>
</div>
    </div>

    <div class='main'>
        <div class='header'>
            <div>
                <h1 style='margin:0'>Category Management</h1>
                <p style='color:#64748b'>Efficiently organize and curate your store's product taxonomy.</p>
            </div>
            <button class='btn-add' onclick='sendToCSharp(""add"")'>+ Add Category</button>
        </div>

        <div class='stats'>
            <div class='card'><small>Total Categories</small><h2>24</h2></div>
            <div class='card'><small>Active Products</small><h2>1,680</h2></div>
            <div class='card'><small>Top Performing</small><h2>Electronics</h2></div>
            <div class='card'><small>Last Updated</small><h2 style='font-size:16px'>2 hours ago</h2></div>
        </div>

        <table>
            <thead>
                <tr><th>CATEGORY ID</th><th>NAME</th><th>DESCRIPTION</th><th>PRODUCTS</th><th>ACTIONS</th></tr>
            </thead>
            <tbody>
                <tr>
                    <td style='color:#94a3b8'>CAT-001</td>
                    <td><b>Electronics</b></td>
                    <td>Consumer electronics, mobile devices, and computers.</td>
                    <td><span class='badge'>450</span></td>
                    <td class='actions'><button onclick='sendToCSharp(""edit"", ""001"")'>✎</button><button onclick='sendToCSharp(""delete"", ""001"")'>🗑</button></td>
                </tr>
                <tr>
                    <td style='color:#94a3b8'>CAT-002</td>
                    <td><b>Home & Kitchen</b></td>
                    <td>Kitchen appliances, furniture, and bedding.</td>
                    <td><span class='badge'>230</span></td>
                    <td class='actions'><button onclick='sendToCSharp(""edit"", ""002"")'>✎</button><button onclick='sendToCSharp(""delete"", ""002"")'>🗑</button></td>
                </tr>
            </tbody>
        </table>
    </div>
    <script>
        function navigate(page) {
            window.chrome.webview.postMessage({ action: 'NAVIGATE', page: page });
        }
    </script>
</body>
</html>";

            // تحميل الكود مباشرة
            webView.CoreWebView2.NavigateToString(htmlContent);

            // ربط حدث استقبال الرسائل
            webView.CoreWebView2.WebMessageReceived += OnWebMessageReceived;
        }
        private void OnWebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            using (JsonDocument doc = JsonDocument.Parse(e.WebMessageAsJson))
            {
                string action = doc.RootElement.GetProperty("action").GetString();
                if (action == "NAVIGATE")
                {
                    string page = doc.RootElement.GetProperty("page").GetString();
                    HandleNavigation(page);
                }
            }
        }
        private void HandleNavigation(string page)
        {
            switch (page)
            {
                case "dashboard":
                    new DashboardForm().Show();
                    this.Hide();
                    break;
                case "products":
                    new ProductForm().Show();
                    this.Hide();
                    break;
                case "orders":
                    new OrderForm().Show();
                    this.Hide();
                    break;
                case "users":
                    new CustomerForm().Show();
                    this.Hide();
                    break;
                case "admin":
                    new AdminForm().Show();
                    this.Hide();
                    break;
                default:
                    MessageBox.Show($"Opening {page} view...");
                    break;
            }
        }
    }
}
