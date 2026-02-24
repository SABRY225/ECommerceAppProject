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
                <style>
                    :root { --sidebar-dark: #1e293b; --bg-light: #f8fafc; --accent: #0f172a; --text-muted: #64748b; }
                    body { font-family: 'Segoe UI', Tahoma, sans-serif; margin: 0; display: flex; height: 100vh; background: var(--bg-light); overflow: hidden; }
                    
                    /* Sidebar */
                    .sidebar { width: 260px; background: var(--sidebar-dark); color: white; display: flex; flex-direction: column; }
                    .sidebar-header { padding: 25px; border-bottom: 1px solid #334155; }
                    .nav-list { list-style: none; padding: 15px; margin: 0; }
                    .nav-item { padding: 12px 15px; margin-bottom: 5px; border-radius: 8px; cursor: pointer; color: #94a3b8; display: flex; align-items: center; transition: 0.2s; }
                    .nav-item:hover { background: #334155; color: white; }
                    .nav-item.active { background: #334155; color: white; border-left: 4px solid #38bdf8; }

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
                <nav class='sidebar'>
                    <div class='sidebar-header'><strong>ADMIN PANEL</strong><br><small style='color:#64748b'>E-Commerce System</small></div>
                    <ul class='nav-list'>
                        <li class='nav-item'>Dashboard</li>
                        <li class='nav-item'>Categories</li>
                        <li class='nav-item active'>Products</li>
                        <li class='nav-item'>Orders</li>
                        <li class='nav-item'>Customers</li>
                    </ul>
                </nav>

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
                    function postToCSharp(action, id) {
                        window.chrome.webview.postMessage({ Action: action, Id: id });
                    }

                    function filterTable(value) {
                        // كود بسيط للفلترة في الواجهة
                        let rows = document.querySelectorAll('tbody tr');
                        rows.forEach(row => {
                            row.style.display = row.innerText.toLowerCase().includes(value.toLowerCase()) ? '' : 'none';
                        });
                    }
                </script>
            </body>
            </html>";

            webView.CoreWebView2.NavigateToString(htmlContent);
            webView.CoreWebView2.WebMessageReceived += WebMessageReceived;
        }
        private void WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            var json = e.WebMessageAsJson;
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                string action = doc.RootElement.GetProperty("Action").GetString();
                string id = doc.RootElement.TryGetProperty("Id", out var idProp) ? idProp.GetString() : "";

                switch (action)
                {
                    case "add_product":
                        MessageBox.Show("فتح نموذج إضافة منتج جديد");
                        break;
                    case "edit":
                        MessageBox.Show("تعديل المنتج: " + id);
                        break;
                    case "delete":
                        DialogResult result = MessageBox.Show($"هل تريد حذف المنتج {id}؟", "تأكيد", MessageBoxButtons.YesNo);
                        if (result == DialogResult.Yes) { /* كود الحذف هنا */ }
                        break;
                }
            }
        }
    }
}
