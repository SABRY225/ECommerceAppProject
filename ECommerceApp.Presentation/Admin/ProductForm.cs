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
using ECommerceApp.Application.Interfaces.Services;

namespace ECommerceApp.Presentation.Admin
{
    public partial class ProductForm : Form
    {
        private WebView2 webView;
        public ProductForm()
        {
            InitializeComponent();
            this.Text = "E-Comm Suite - Product Management";
            this.WindowState = FormWindowState.Maximized;
            InitializeWebView();
        }
        private async void InitializeWebView()
        {
            webView = new WebView2 { Dock = DockStyle.Fill };
            this.Controls.Add(webView);

            // تهيئة البيئة
            await webView.EnsureCoreWebView2Async(null);

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
        :root { --sidebar-dark: #1e293b; --bg-light: #f8fafc; --accent: #0f172a; --text-muted: #64748b; }
        
        /* Sidebar */
        .sidebar { width: 260px; background: var(--sidebar-bg); color: white; display: flex; flex-direction: column; }
        .sidebar-header { padding: 30px 20px; border-bottom: 1px solid rgba(255,255,255,0.1); }
        .nav-link { color: var(--text-light); padding: 15px 25px; text-decoration: none; display: flex; align-items: center; gap: 15px; transition: 0.3s; }
        .nav-link:hover, .nav-link.active { background: rgba(255,255,255,0.1); color: white; }
        .user-profile { margin-top: auto; padding: 20px; background: rgba(0,0,0,0.2); display: flex; align-items: center; gap: 12px; }

        /* Main Container */
        .main { flex: 1; display: flex; flex-direction: column; overflow-y: auto; padding: 40px; }
        .header { display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 30px; }
        .header h1 { margin: 0; font-size: 24px; color: #1e293b; }
        .header p { margin: 5px 0 0; color: var(--text-muted); }
        .btn-add { background: var(--accent); color: white; border: none; padding: 12px 24px; border-radius: 8px; cursor: pointer; font-weight: 600; display: flex; align-items: center; gap: 8px; }

        /* Search Bar */
        .search-container { background: white; border: 1px solid #e2e8f0; border-radius: 10px; padding: 12px 20px; margin-bottom: 25px; display: flex; align-items: center; }
        .search-container input { border: none; outline: none; width: 100%; font-size: 14px; color: #334155; }

        /* Table Styles */
        .table-container { background: white; border-radius: 12px; border: 1px solid #e2e8f0; overflow: hidden; box-shadow: 0 4px 6px -1px rgba(0,0,0,0.05); }
        table { width: 100%; border-collapse: collapse; }
        th { background: #f8fafc; padding: 15px; text-align: left; font-size: 12px; color: var(--text-muted); text-transform: uppercase; letter-spacing: 0.05em; border-bottom: 1px solid #e2e8f0; }
        td { padding: 15px; border-bottom: 1px solid #f1f5f9; vertical-align: middle; color: #334155; font-size: 14px; }
        
        /* Product Info */
        .product-cell { display: flex; align-items: center; gap: 15px; }
        .product-img { width: 48px; height: 48px; border-radius: 8px; background: #f1f5f9; object-fit: cover; border: 1px solid #e2e8f0; }
        .sku { font-size: 11px; color: #94a3b8; display: block; }
        
        /* Badges */
        .badge { padding: 4px 12px; border-radius: 20px; font-size: 12px; font-weight: 500; }
        .badge-electronics { background: #eff6ff; color: #2563eb; }
        .badge-furniture { background: #fff7ed; color: #ea580c; }
        .badge-accessories { background: #f8fafc; color: #475569; border: 1px solid #e2e8f0; }

        /* Action Buttons */
        .actions button { background: none; border: none; cursor: pointer; padding: 5px; color: #94a3b8; transition: 0.2s; }
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
            <a href='#' class='nav-link' onclick='navigate(""categories"")'><i class='bi bi-tags'></i> Categories</a>
            <a href='#' class='nav-link active' onclick='navigate(""products"")'><i class='bi bi-box-seam'></i> Products</a>
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


    <main class='main'>
        <header class='header'>
            <div>
                <h1>Product Management</h1>
                <p>Manage and update your central product catalog.</p>
            </div>
            <button class='btn-add' onclick='postToCSharp(""add_product"", null)'>+ Add New Product</button>
        </header>

        <div class='search-container'>
            <input type='text' placeholder='Search products by name, SKU or category...' onkeyup='filterTable(this.value)'>
        </div>

        <div class='table-container'>
            <table>
                <thead>
                    <tr>
                        <th>Image</th>
                        <th>Product Name</th>
                        <th>Category</th>
                        <th>Price</th>
                        <th>Stock Level</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td><img src='https://cdn-icons-png.flaticon.com/512/428/428911.png' class='product-img'></td>
                        <td><b>MacBook Pro M2</b><span class='sku'>SKU: LAP-00231</span></td>
                        <td><span class='badge badge-electronics'>Electronics</span></td>
                        <td>$1,999.00</td>
                        <td>42 units</td>
                        <td class='actions'>
                            <button onclick='postToCSharp(""edit"", ""LAP-00231"")'>✎</button>
                            <button onclick='postToCSharp(""delete"", ""LAP-00231"")'>🗑</button>
                        </td>
                    </tr>
                    <tr>
                        <td><img src='https://cdn-icons-png.flaticon.com/512/0/191.png' class='product-img'></td>
                        <td><b>iPhone 14 Pro</b><span class='sku'>SKU: PHN-99812</span></td>
                        <td><span class='badge badge-electronics'>Electronics</span></td>
                        <td>$999.00</td>
                        <td>42 units</td>
                        <td class='actions'>
                            <button>✎</button><button>🗑</button>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </main>

    <script>
        function navigate(page) {
            window.chrome.webview.postMessage({ action: 'NAVIGATE', page: page });
        }
    </script>
</body>
</html>";

            webView.CoreWebView2.NavigateToString(htmlContent);
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
        private readonly ICategoryService _categoryService;

        private void HandleNavigation(string page)
        {
            // هذا الجزء يربط القائمة الجانبية بفتح الفورمز التي قمنا ببرمجتها سابقاً
            switch (page)
            {
                case "categories":
                    new CategoryForm(_categoryService).Show();
                    this.Hide();
                    break;
                case "orders":
                    new OrderForm().Show();
                    this.Hide();
                    break;
                //case "dashboard":
                //    new DashboardForm().Show();
                //    this.Hide();
                //    break;
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
