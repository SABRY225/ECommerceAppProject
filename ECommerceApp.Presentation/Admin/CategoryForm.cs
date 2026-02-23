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
                <style>
                    :root { --sidebar-bg: #1e293b; --bg-main: #f8fafc; --primary: #0f172a; }
                    body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 0; display: flex; height: 100vh; background: var(--bg-main); }
                    
                    /* Sidebar */
                    .sidebar { width: 250px; background: var(--sidebar-bg); color: #94a3b8; display: flex; flex-direction: column; padding: 20px; }
                    .sidebar h2 { color: white; font-size: 18px; margin-bottom: 30px; border-bottom: 1px solid #334155; padding-bottom: 15px; }
                    .nav-item { padding: 12px; margin-bottom: 5px; cursor: pointer; border-radius: 8px; transition: 0.3s; }
                    .nav-item:hover { background: #334155; color: white; }
                    .nav-item.active { background: #334155; color: white; border-left: 4px solid #38bdf8; }

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
                    <h2>ADMIN PANEL</h2>
                    <div class='nav-item'>Dashboard</div>
                    <div class='nav-item active'>Categories</div>
                    <div class='nav-item'>Products</div>
                    <div class='nav-item'>Orders</div>
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
                    function sendToCSharp(action, id) {
                        const data = { action: action, id: id };
                        window.chrome.webview.postMessage(data);
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
            // فك تشفير الرسالة القادمة من HTML
            var json = e.WebMessageAsJson;
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                string action = doc.RootElement.GetProperty("action").GetString();

                if (action == "add")
                {
                    MessageBox.Show("سيتم فتح واجهة إضافة تصنيف جديد.");
                }
                else if (action == "delete")
                {
                    string id = doc.RootElement.GetProperty("id").GetString();
                    MessageBox.Show($"هل أنت متأكد من حذف التصنيف ذو الرقم: {id}؟");
                }
            }
        }
    }
}
