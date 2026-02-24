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
                <style>
                    :root { --sidebar-bg: #1e3a5a; --bg: #f8fafc; --text-dark: #1e293b; --text-muted: #64748b; }
                    body { font-family: 'Segoe UI', system-ui, sans-serif; margin: 0; display: flex; height: 100vh; background: var(--bg); overflow: hidden; }
                    
                    /* Sidebar Layout */
                    .sidebar { width: 260px; background: var(--sidebar-bg); color: white; display: flex; flex-direction: column; }
                    .sidebar-brand { padding: 25px; font-size: 18px; font-weight: bold; border-bottom: 1px solid rgba(255,255,255,0.1); }
                    .nav-menu { list-style: none; padding: 20px 10px; margin: 0; }
                    .nav-link { padding: 12px 15px; border-radius: 8px; color: #cbd5e1; cursor: pointer; display: flex; align-items: center; margin-bottom: 5px; }
                    .nav-link:hover { background: rgba(255,255,255,0.05); color: white; }
                    .nav-link.active { background: #2563eb; color: white; }

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
                <aside class='sidebar'>
                    <div class='sidebar-brand'>AdminPanel<br><small style='font-weight:normal;opacity:0.6'>E-Commerce Management</small></div>
                    <div class='nav-menu'>
                        <div class='nav-link'>Dashboard</div>
                        <div class='nav-link'>Categories</div>
                        <div class='nav-link'>Products</div>
                        <div class='nav-link active'>Orders</div>
                        <div class='nav-link'>Customers</div>
                    </div>
                </aside>

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
                    function searchTable() {
                        let input = document.getElementById('orderSearch').value.toLowerCase();
                        let rows = document.getElementById('ordersTable').getElementsByTagName('tr');
                        for (let i = 1; i < rows.length; i++) {
                            let text = rows[i].innerText.toLowerCase();
                            rows[i].style.display = text.includes(input) ? '' : 'none';
                        }
                    }

                    function sendAction(orderId) {
                        window.chrome.webview.postMessage({ action: 'view_details', id: orderId });
                    }
                </script>
            </body>
            </html>";

            webView.CoreWebView2.NavigateToString(htmlContent);
            webView.CoreWebView2.WebMessageReceived += OnMessageReceived;
        }

        private void OnMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            var json = e.WebMessageAsJson;
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                string action = doc.RootElement.GetProperty("action").GetString();
                string id = doc.RootElement.GetProperty("id").GetString();

                if (action == "view_details")
                {
                    MessageBox.Show($"جاري عرض تفاصيل الطلب رقم: {id}", "نظام الطلبات");
                }
            }
        }
    }
}
