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
    public partial class AdminForm : Form
    {
        private WebView2 webView;
        public AdminForm()
        {
            InitializeComponent();
            this.Text = "E-Comm Suite - Admin Management";
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
                    :root { --sidebar-bg: #1e3a5a; --bg: #f8fafc; --primary: #1e293b; --text-muted: #64748b; }
                    body { font-family: 'Segoe UI', Tahoma, sans-serif; margin: 0; display: flex; height: 100vh; background: var(--bg); overflow: hidden; }
                    
                    /* Sidebar Layout */
                    .sidebar { width: 260px; background: var(--sidebar-bg); color: white; display: flex; flex-direction: column; }
                    .sidebar-brand { padding: 25px; font-size: 18px; font-weight: bold; border-bottom: 1px solid rgba(255,255,255,0.1); }
                    .nav-menu { list-style: none; padding: 20px 10px; margin: 0; }
                    .nav-link { padding: 12px 15px; border-radius: 8px; color: #cbd5e1; cursor: pointer; display: flex; align-items: center; margin-bottom: 5px; }
                    .nav-link:hover { background: rgba(255,255,255,0.05); color: white; }
                    .nav-link.active { background: rgba(255,255,255,0.1); color: white; border-left: 4px solid #38bdf8; }

                    /* Main Container */
                    .main { flex: 1; padding: 40px; overflow-y: auto; }
                    .header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 30px; }
                    .header h1 { margin: 0; font-size: 24px; color: var(--primary); }
                    .btn-add { background: #0f172a; color: white; border: none; padding: 10px 20px; border-radius: 8px; cursor: pointer; font-weight: bold; }

                    /* Search Bar */
                    .search-bar { background: white; border: 1px solid #e2e8f0; border-radius: 10px; padding: 12px 20px; margin-bottom: 25px; display: flex; align-items: center; gap: 10px; }
                    .search-bar input { border: none; outline: none; width: 100%; font-size: 14px; }

                    /* Table Styles */
                    .card-table { background: white; border-radius: 12px; border: 1px solid #e2e8f0; overflow: hidden; box-shadow: 0 4px 6px -1px rgba(0,0,0,0.05); }
                    table { width: 100%; border-collapse: collapse; }
                    th { text-align: left; padding: 15px; background: #f8fafc; color: var(--text-muted); font-size: 12px; text-transform: uppercase; border-bottom: 1px solid #e2e8f0; }
                    td { padding: 15px; border-bottom: 1px solid #f1f5f9; }

                    /* Administrator Cell */
                    .admin-info { display: flex; align-items: center; gap: 12px; }
                    .admin-img { width: 40px; height: 40px; border-radius: 50%; object-fit: cover; background: #e2e8f0; }
                    .admin-name { display: block; font-weight: 600; color: #1e293b; }
                    .admin-email { font-size: 12px; color: var(--text-muted); }

                    /* Role Badges */
                    .badge { padding: 4px 10px; border-radius: 20px; font-size: 11px; font-weight: 600; }
                    .badge-super { background: #f1f5f9; color: #475569; border: 1px solid #e2e8f0; }
                    .badge-editor { background: #eff6ff; color: #2563eb; }
                    .badge-viewer { background: #f8fafc; color: #94a3b8; }

                    .last-login { color: var(--text-muted); font-size: 13px; }
                    .actions { color: #94a3b8; font-size: 18px; cursor: pointer; }
                </style>
            </head>
            <body>
                <nav class='sidebar'>
                    <div class='sidebar-brand'>AdminPanel</div>
                    <div class='nav-menu'>
                        <div class='nav-link'>Dashboard</div>
                        <div class='nav-link'>Orders</div>
                        <div class='nav-link'>Products</div>
                        <div class='nav-link'>Customers</div>
                        <div class='nav-link active'>Admin Management</div>
                    </div>
                </nav>

                <main class='main'>
                    <div class='header'>
                        <div>
                            <h1>Administrator Management</h1>
                            <p style='color: #64748b; margin: 5px 0;'>Manage team members and their access levels.</p>
                        </div>
                        <button class='btn-add' onclick='sendAction(""add"")'>+ Add New Admin</button>
                    </div>

                    <div class='search-bar'>
                        <span>🔍</span>
                        <input type='text' placeholder='Search admins by name, email or role...' onkeyup='filterAdmins(this.value)'>
                    </div>

                    <div class='card-table'>
                        <table id='adminTable'>
                            <thead>
                                <tr>
                                    <th>Administrator</th>
                                    <th>Role</th>
                                    <th>Last Login</th>
                                    <th>Actions</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td>
                                        <div class='admin-info'>
                                            <img src='https://i.pravatar.cc/150?u=sarah' class='admin-img'>
                                            <div><span class='admin-name'>Sarah Jenkins</span><span class='admin-email'>sarah.j@shopadmin.com</span></div>
                                        </div>
                                    </td>
                                    <td><span class='badge badge-super'>Super Admin</span></td>
                                    <td class='last-login'>2 hours ago</td>
                                    <td class='actions' onclick='sendAction(""edit"", ""Sarah"")'>✎ 🗑</td>
                                </tr>
                                <tr>
                                    <td>
                                        <div class='admin-info'>
                                            <img src='https://i.pravatar.cc/150?u=marcus' class='admin-img'>
                                            <div><span class='admin-name'>Marcus Thorne</span><span class='admin-email'>m.thorne@shopadmin.com</span></div>
                                        </div>
                                    </td>
                                    <td><span class='badge badge-editor'>Editor</span></td>
                                    <td class='last-login'>Yesterday, 14:20</td>
                                    <td class='actions'>✎ 🗑</td>
                                </tr>
                                <tr>
                                    <td>
                                        <div class='admin-info'>
                                            <img src='https://i.pravatar.cc/150?u=linda' class='admin-img'>
                                            <div><span class='admin-name'>Linda Wu</span><span class='admin-email'>linda.wu@shopadmin.com</span></div>
                                        </div>
                                    </td>
                                    <td><span class='badge badge-viewer'>Viewer</span></td>
                                    <td class='last-login'>1 week ago</td>
                                    <td class='actions'>✎ 🗑</td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </main>

                <script>
                    function sendAction(type, name) {
                        window.chrome.webview.postMessage({ action: type, user: name });
                    }

                    function filterAdmins(query) {
                        let rows = document.querySelectorAll('#adminTable tbody tr');
                        rows.forEach(row => {
                            row.style.display = row.innerText.toLowerCase().includes(query.toLowerCase()) ? '' : 'none';
                        });
                    }
                </script>
            </body>
            </html>";

            webView.CoreWebView2.NavigateToString(htmlContent);
            webView.CoreWebView2.WebMessageReceived += OnWebMessageReceived;
        }

        private void OnWebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            var json = e.WebMessageAsJson;
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                string action = doc.RootElement.GetProperty("action").GetString();
                if (action == "add") MessageBox.Show("فتح واجهة إضافة مسؤول جديد");
            }
        }
    }
}
