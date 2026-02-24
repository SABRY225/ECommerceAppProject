using ECommerceApp.Application.Interfaces.Services;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ECommerceApp.Presentation.Admin
{
    public partial class DashboardForm : Form
    {
        private WebView2 webView;
        private readonly ICategoryService _categoryService;
        public DashboardForm(ICategoryService categoryService)
        {
            InitializeComponent();
            this.Text = "E-Comm Suite - Administrator Dashboard";
            this.WindowState = FormWindowState.Maximized;
            _categoryService = categoryService;

            InitializeWebView();
        }
        private async void InitializeWebView()
        {
            webView = new WebView2 { Dock = DockStyle.Fill };
            this.Controls.Add(webView);

            await webView.EnsureCoreWebView2Async(null);
            webView.CoreWebView2.WebMessageReceived += OnWebMessageReceived;

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

        /* Main Content */
        .content { flex: 1; overflow-y: auto; padding: 40px; }
        .stat-card { background: white; border-radius: 12px; padding: 25px; border: none; box-shadow: 0 4px 12px rgba(0,0,0,0.03); }
        .stat-icon { width: 50px; height: 50px; border-radius: 10px; background: #f0f4f8; display: flex; align-items: center; justify-content: center; font-size: 1.5rem; color: var(--sidebar-bg); }
        
        /* Table */
        .table-card { background: white; border-radius: 12px; padding: 25px; margin-top: 30px; }
        .status-badge { font-size: 0.75rem; padding: 5px 12px; border-radius: 20px; font-weight: bold; }
        .status-paid { background: #e6f9f0; color: #00b050; }
        .status-processing { background: #fff4e6; color: #ff9900; }
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
            <a href='#' class='nav-link active' onclick='navigate(""dashboard"")'><i class='bi bi-grid'></i> Dashboard</a>
            <a href='#' class='nav-link' onclick='navigate(""categories"")'><i class='bi bi-tags'></i> Categories</a>
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

    <div class='content'>
        <h4 class='fw-bold mb-4'>Administrator Dashboard</h4>
        
        <div class='row g-4'>
            <div class='col-md-3'>
                <div class='stat-card d-flex align-items-center justify-content-between'>
                    <div><div class='text-muted small'>TOTAL PRODUCTS</div><h3 class='fw-bold m-0'>1,240</h3></div>
                    <div class='stat-icon'><i class='bi bi-box'></i></div>
                </div>
            </div>
            <div class='col-md-3'>
                <div class='stat-card d-flex align-items-center justify-content-between'>
                    <div><div class='text-muted small'>TOTAL CATEGORIES</div><h3 class='fw-bold m-0'>42</h3></div>
                    <div class='stat-icon'><i class='bi bi-diagram-3'></i></div>
                </div>
            </div>
            <div class='col-md-3'>
                <div class='stat-card d-flex align-items-center justify-content-between'>
                    <div><div class='text-muted small'>TOTAL ORDERS</div><h3 class='fw-bold m-0'>856</h3></div>
                    <div class='stat-icon'><i class='bi bi-bag'></i></div>
                </div>
            </div>
            <div class='col-md-3'>
                <div class='stat-card d-flex align-items-center justify-content-between'>
                    <div><div class='text-muted small'>TOTAL SALES</div><h3 class='fw-bold m-0'>$124,500</h3></div>
                    <div class='stat-icon'><i class='bi bi-cash-stack'></i></div>
                </div>
            </div>
        </div>

        <div class='table-card'>
            <h6 class='fw-bold mb-4'><i class='bi bi-list-ul me-2'></i> Recent Orders Data</h6>
            <table class='table align-middle'>
                <thead class='table-light'>
                    <tr style='font-size: 0.75rem; color: #888;'>
                        <th>ORDER ID</th><th>CUSTOMER</th><th>DATE</th><th>TOTAL AMOUNT</th><th>STATUS</th><th>ACTIONS</th>
                    </tr>
                </thead>
                <tbody style='font-size: 0.85rem;'>
                    <tr>
                        <td class='fw-bold'>#ORD-7721</td>
                        <td>Jane Cooper</td>
                        <td>Oct 24, 2023</td>
                        <td class='fw-bold'>$540.00</td>
                        <td><span class='status-badge status-paid'>PAID</span></td>
                        <td><i class='bi bi-three-dots cursor-pointer'></i></td>
                    </tr>
                    <tr>
                        <td class='fw-bold'>#ORD-7722</td>
                        <td>Cody Wilson</td>
                        <td>Oct 25, 2023</td>
                        <td class='fw-bold'>$1,240.50</td>
                        <td><span class='status-badge status-processing'>PROCESSING</span></td>
                        <td><i class='bi bi-three-dots cursor-pointer'></i></td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>

    <script>
        function navigate(page) {
            window.chrome.webview.postMessage({ action: 'NAVIGATE', page: page });
        }
    </script>
</body>
</html>";

            webView.NavigateToString(htmlContent);
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
                case "categories":
                    new CategoryForm(_categoryService).Show();
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
