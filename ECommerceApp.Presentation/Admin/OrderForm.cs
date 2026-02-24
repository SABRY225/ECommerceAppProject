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
    public partial class OrderForm : Form
    {
        private WebView2 webView;
        public OrderForm()
        {
            InitializeComponent();
            this.Text = "E-Comm Suite - Order Management";
            this.WindowState = FormWindowState.Maximized;
            InitializeWebView();
        }
        private async void InitializeWebView()
        {
            webView = new WebView2 { Dock = DockStyle.Fill };
            this.Controls.Add(webView);

            await webView.EnsureCoreWebView2Async(null);

            // كود الـ HTML والـ CSS والـ JS مدمج لضمان عمل الواجهة فوراً
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

         /* Content Area */
        .container { flex: 1; padding: 40px; overflow-y: auto; }
        .header { margin-bottom: 30px; }
        .header h1 { margin: 0; color: var(--text-dark); font-size: 24px; }
        .header p { margin: 5px 0 0; color: var(--text-muted); }

        /* Search Bar */
        .search-box { background: white; border: 1px solid #e2e8f0; border-radius: 10px; padding: 12px 20px; margin-bottom: 25px; display: flex; gap: 10px; }
        .search-box input { border: none; outline: none; width: 100%; font-size: 14px; }

        /* Orders Table */
        .table-card { background: white; border-radius: 12px; border: 1px solid #e2e8f0; box-shadow: 0 4px 6px -1px rgba(0,0,0,0.05); }
        table { width: 100%; border-collapse: collapse; }
        th { text-align: left; padding: 18px; background: #f8fafc; color: var(--text-muted); font-size: 12px; text-transform: uppercase; border-bottom: 1px solid #e2e8f0; }
        td { padding: 18px; border-bottom: 1px solid #f1f5f9; font-size: 14px; color: var(--text-dark); }
        
        /* Customer Info */
        .cust-name { font-weight: 600; display: block; }
        .cust-email { font-size: 12px; color: var(--text-muted); }

        /* Status Badges */
        .status { padding: 5px 12px; border-radius: 6px; font-size: 12px; font-weight: 500; display: inline-flex; align-items: center; gap: 6px; }
        .status::before { content: ''; width: 6px; height: 6px; border-radius: 50%; }
        
        .status-processing { background: #fef3c7; color: #d97706; }
        .status-processing::before { background: #d97706; }
        
        .status-shipped { background: #e0e7ff; color: #4338ca; }
        .status-shipped::before { background: #4338ca; }
        
        .status-delivered { background: #dcfce7; color: #15803d; }
        .status-delivered::before { background: #15803d; }

        .action-dots { color: #94a3b8; cursor: pointer; font-weight: bold; font-size: 18px; }
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
            <a href='#' class='nav-link active' onclick='navigate(""orders"")'><i class='bi bi-cart'></i> Orders</a>
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


    <main class='container'>
        <div class='header'>
            <h1>Order Management</h1>
            <p>Manage and track customer purchases in real-time.</p>
        </div>

        <div class='search-box'>
            <span>🔍</span>
            <input type='text' id='orderSearch' placeholder='Search by Order ID, Customer Name...' onkeyup='searchTable()'>
        </div>

        <div class='table-card'>
            <table id='ordersTable'>
                <thead>
                    <tr>
                        <th>Order ID</th>
                        <th>Customer</th>
                        <th>Date</th>
                        <th>Total Amount</th>
                        <th>Status</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td><b>#3RD-1024</b></td>
                        <td><span class='cust-name'>John Doe</span><span class='cust-email'>john.doe@example.com</span></td>
                        <td>Oct 24, 2023</td>
                        <td><b>$124.50</b></td>
                        <td><span class='status status-processing'>Processing</span></td>
                        <td class='action-dots' onclick='sendAction(""#3RD-1024"")'>⋮</td>
                    </tr>
                    <tr>
                        <td><b>#3RD-1025</b></td>
                        <td><span class='cust-name'>Jane Smith</span><span class='cust-email'>jane.smith@web.com</span></td>
                        <td>Oct 24, 2023</td>
                        <td><b>$89.20</b></td>
                        <td><span class='status status-shipped'>Shipped</span></td>
                        <td class='action-dots'>⋮</td>
                    </tr>
                    <tr>
                        <td><b>#3RD-1026</b></td>
                        <td><span class='cust-name'>Robert Brown</span><span class='cust-email'>robert.b@domain.org</span></td>
                        <td>Oct 23, 2023</td>
                        <td><b>$210.00</b></td>
                        <td><span class='status status-delivered'>Delivered</span></td>
                        <td class='action-dots'>⋮</td>
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
        private void HandleNavigation(string page)
        {
            // هذا الجزء يربط القائمة الجانبية بفتح الفورمز التي قمنا ببرمجتها سابقاً
            switch (page)
            {
                case "categories":
                    new CategoryForm().Show();
                    this.Hide();
                    break;
                case "products":
                    new ProductForm().Show();
                    this.Hide();
                    break;
                case "dashboard":
                    new DashboardForm().Show();
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
