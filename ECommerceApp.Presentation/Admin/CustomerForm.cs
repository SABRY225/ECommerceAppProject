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
                <style>
                    :root { --sidebar-bg: #1e3a5a; --bg: #f8fafc; --text-main: #1e293b; --text-muted: #64748b; }
                    body { font-family: 'Segoe UI', system-ui, sans-serif; margin: 0; display: flex; height: 100vh; background: var(--bg); overflow: hidden; }
                    
                    /* Sidebar Layout */
                    .sidebar { width: 260px; background: var(--sidebar-bg); color: white; display: flex; flex-direction: column; }
                    .sidebar-brand { padding: 25px; font-size: 18px; font-weight: bold; border-bottom: 1px solid rgba(255,255,255,0.1); }
                    .nav-menu { list-style: none; padding: 20px 10px; margin: 0; }
                    .nav-link { padding: 12px 15px; border-radius: 8px; color: #cbd5e1; cursor: pointer; display: flex; align-items: center; margin-bottom: 5px; }
                    .nav-link:hover { background: rgba(255,255,255,0.05); color: white; }
                    .nav-link.active { background: rgba(255,255,255,0.1); color: white; border-left: 4px solid #38bdf8; }

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
                <nav class='sidebar'>
                    <div class='sidebar-brand'>AdminPanel<br><small style='font-weight:normal; font-size: 11px; opacity: 0.6;'>E-Commerce Management</small></div>
                    <div class='nav-menu'>
                        <div class='nav-link'>Dashboard</div>
                        <div class='nav-link'>Categories</div>
                        <div class='nav-link'>Products</div>
                        <div class='nav-link'>Orders</div>
                        <div class='nav-link active'>Customers</div>
                    </div>
                </nav>

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
                    function filterCustomers() {
                        let input = document.getElementById('customerSearch').value.toLowerCase();
                        let rows = document.getElementById('customerTable').getElementsByTagName('tr');
                        
                        for (let i = 1; i < rows.length; i++) {
                            let text = rows[i].innerText.toLowerCase();
                            rows[i].style.display = text.includes(input) ? '' : 'none';
                        }
                    }

                    // دالة لاستقبال بيانات من C# وتحديث الجدول (اختياري)
                    window.chrome.webview.addEventListener('message', event => {
                        console.log('Data received from C#:', event.data);
                    });
                </script>
            </body>
            </html>";

            webView.CoreWebView2.NavigateToString(htmlContent);
        }
    }
}
