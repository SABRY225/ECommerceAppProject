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
    public partial class CustomerForm : Form
    {
        private WebView2 webView;

        public CustomerForm()
        {
            InitializeComponent();
            this.Text = "E-Comm Suite - Customer Management";
            this.WindowState = FormWindowState.Maximized;
            InitializeWebView();
        }
        private async void InitializeWebView()
        {
            webView = new WebView2 { Dock = DockStyle.Fill };
            this.Controls.Add(webView);

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

           /* Main Content */
        .main { flex: 1; padding: 40px; overflow-y: auto; }
        .header h1 { margin: 0; color: var(--text-main); font-size: 24px; }
        
        /* Search Bar */
        .search-container { background: white; border: 1px solid #e2e8f0; border-radius: 10px; padding: 12px 20px; margin: 25px 0; display: flex; align-items: center; gap: 10px; }
        .search-container input { border: none; outline: none; width: 100%; font-size: 14px; color: var(--text-main); }

        /* Table Styles */
        .table-card { background: white; border-radius: 12px; border: 1px solid #e2e8f0; box-shadow: 0 4px 6px -1px rgba(0,0,0,0.05); overflow: hidden; }
        table { width: 100%; border-collapse: collapse; }
        th { text-align: left; padding: 18px; background: #f8fafc; color: var(--text-muted); font-size: 12px; text-transform: uppercase; border-bottom: 1px solid #e2e8f0; }
        td { padding: 18px; border-bottom: 1px solid #f1f5f9; vertical-align: middle; }

        /* Customer Avatar & Info */
        .customer-cell { display: flex; align-items: center; gap: 15px; }
        .avatar { width: 40px; height: 40px; border-radius: 50%; background: #e2e8f0; display: flex; align-items: center; justify-content: center; font-weight: bold; color: #475569; font-size: 14px; }
        .name { display: block; font-weight: 600; color: var(--text-main); }
        .email { font-size: 12px; color: var(--text-muted); }
        
        .join-date { color: var(--text-muted); font-size: 14px; }
        .total-orders { font-weight: bold; text-align: center; color: var(--text-main); }
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
            <a href='#' class='nav-link' onclick='navigate(""products"")'><i class='bi bi-box-seam'></i> Products</a>
            <a href='#' class='nav-link' onclick='navigate(""orders"")'><i class='bi bi-cart'></i> Orders</a>
            <a href='#' class='nav-link active' onclick='navigate(""users"")'><i class='bi bi-people'></i> Users</a>
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
        <div class='header'>
            <h1>Customer Management</h1>
        </div>

        <div class='search-container'>
            <span>🔍</span>
            <input type='text' id='customerSearch' placeholder='Search by name, email or ID...' onkeyup='filterCustomers()'>
        </div>

        <div class='table-card'>
            <table id='customerTable'>
                <thead>
                    <tr>
                        <th>Customer</th>
                        <th>Join Date</th>
                        <th style='text-align: center;'>Total Orders</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>
                            <div class='customer-cell'>
                                <div class='avatar'>JC</div>
                                <div><span class='name'>Jane Cooper</span><span class='email'>jane.cooper@example.com</span></div>
                            </div>
                        </td>
                        <td class='join-date'>Oct 12, 2023</td>
                        <td class='total-orders'>12</td>
                    </tr>
                    <tr>
                        <td>
                            <div class='customer-cell'>
                                <div class='avatar'>CF</div>
                                <div><span class='name'>Cody Fisher</span><span class='email'>cody.fisher@example.com</span></div>
                            </div>
                        </td>
                        <td class='join-date'>Sep 28, 2023</td>
                        <td class='total-orders'>5</td>
                    </tr>
                    <tr>
                        <td>
                            <div class='customer-cell'>
                                <div class='avatar'>EH</div>
                                <div><span class='name'>Esther Howard</span><span class='email'>esther.howard@example.com</span></div>
                            </div>
                        </td>
                        <td class='join-date'>Sep 15, 2023</td>
                        <td class='total-orders'>24</td>
                    </tr>
                    <tr>
                        <td>
                            <div class='customer-cell'>
                                <div class='avatar'>JW</div>
                                <div><span class='name'>Jenny Wilson</span><span class='email'>jenny.wilson@example.com</span></div>
                            </div>
                        </td>
                        <td class='join-date'>Aug 30, 2023</td>
                        <td class='total-orders'>8</td>
                    </tr>
                    <tr>
                        <td>
                            <div class='customer-cell'>
                                <div class='avatar'>KW</div>
                                <div><span class='name'>Kristin Watson</span><span class='email'>kristin.watson@example.com</span></div>
                            </div>
                        </td>
                        <td class='join-date'>Aug 12, 2023</td>
                        <td class='total-orders'>3</td>
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
            switch (page)
            {
                //case "dashboard":
                //    new DashboardForm().Show();
                //    this.Hide();
                //    break;
                case "products":
                    new ProductForm().Show();
                    this.Hide();
                    break;
                case "orders":
                    new OrderForm().Show();
                    this.Hide();
                    break;
                case "categories":
                    new CategoryForm(_categoryService).Show();
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
