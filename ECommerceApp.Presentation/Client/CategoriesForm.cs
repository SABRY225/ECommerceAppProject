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

namespace ECommerceApp.Presentation.Client
{
    public partial class CategoriesForm : Form
    {
        private WebView2 webView;
        public CategoriesForm()
        { 
            InitializeComponent();
            this.Text = "E-Comm Suite - Categories";
            this.WindowState = FormWindowState.Maximized;
            InitializeWebView();
        }
        private async void InitializeWebView()
        {
            webView = new WebView2();
            webView.Dock = DockStyle.Fill;
            this.Controls.Add(webView);

            // تهيئة محرك WebView2
            await webView.EnsureCoreWebView2Async(null);

            // استقبال الرسائل من JavaScript
            webView.CoreWebView2.WebMessageReceived += OnWebMessageReceived;

            string htmlContent = @"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css' rel='stylesheet'>
    <link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.0/font/bootstrap-icons.css'>
    <style>
        :root { --dark-blue: #1e3a58; --light-gray: #f8f9fa; }
        body { background-color: #fcfcfc; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; padding-bottom: 100px; }
        
        /* Navbar */
        .navbar { background: white; border-bottom: 1px solid #eee; padding: 15px 30px; }
        .user-profile { display: flex; align-items: center; gap: 10px; }

        /* Cards Grid */
        .category-card { 
            background: white; border-radius: 12px; overflow: hidden; 
            box-shadow: 0 4px 12px rgba(0,0,0,0.05); transition: 0.3s; 
            cursor: pointer; border: 1px solid transparent; height: 100%;
        }
        .category-card:hover { transform: translateY(-8px); box-shadow: 0 12px 20px rgba(0,0,0,0.1); border-color: var(--dark-blue); }
        .category-card img { width: 100%; height: 220px; object-fit: cover; background: #eee; }
        .category-title { padding: 20px; text-align: center; font-weight: bold; font-size: 0.9rem; text-transform: uppercase; color: #333; }

        /* Bottom Bar */
        .bottom-bar { 
            position: fixed; bottom: 0; left: 0; right: 0; background: white; 
            padding: 15px 40px; border-top: 1px solid #eee; display: flex; 
            justify-content: space-between; align-items: center; z-index: 1000;
        }
        .btn-pay { background: var(--dark-blue); color: white; padding: 10px 30px; border-radius: 8px; border: none; font-weight: 500; }
        .cart-badge { position: relative; display: inline-block; }
        .badge-count { position: absolute; top: -5px; right: -10px; background: var(--dark-blue); color: white; border-radius: 50%; padding: 2px 6px; font-size: 0.7rem; }
    </style>
</head>
<body>

    <nav class='navbar d-flex justify-content-between align-items-center shadow-sm'>
        <div class='d-flex align-items-center gap-2'>
            <div class='bg-dark text-white p-2 rounded'><i class='bi bi-shop'></i></div>
            <span class='fw-bold fs-5'>E-Comm Suite</span>
        </div>
        <div class='d-flex gap-4 align-items-center'>
            <a href='#' class='text-decoration-none text-dark small fw-bold'>My Orders</a>
            <div class='bg-light p-2 rounded'><i class='bi bi-box'></i></div>
            <div class='user-profile'>
                <span class='small fw-bold'>Ahmed Sabry</span>
                <div class='bg-light rounded-circle p-2'><i class='bi bi-person'></i></div>
            </div>
        </div>
    </nav>

    <div class='container mt-5'>
        <h2 class='text-center fw-bold mb-5'>Browse By Category</h2>
        
        <div class='row g-4'>
            <div class='col-12 col-sm-6 col-md-4 col-lg-3'>
                <div class='category-card' onclick='onCategoryClick(""Perfumes"")'>
                    <img src='https://images.unsplash.com/photo-1541643600914-78b084683601?auto=format&fit=crop&w=400' alt='Perfumes'>
                    <div class='category-title'>Perfumes and Cosmetics</div>
                </div>
            </div>

            <div class='col-12 col-sm-6 col-md-4 col-lg-3'>
                <div class='category-card' onclick='onCategoryClick(""Home"")'>
                    <img src='https://images.unsplash.com/photo-1556911220-e15224bbbe39?auto=format&fit=crop&w=400' alt='Home'>
                    <div class='category-title'>Home and Kitchen</div>
                </div>
            </div>

            <div class='col-12 col-sm-6 col-md-4 col-lg-3'>
                <div class='category-card' onclick='onCategoryClick(""Fashion"")'>
                    <img src='https://images.unsplash.com/photo-1434389677669-e08b4cac3105?auto=format&fit=crop&w=400' alt='Fashion'>
                    <div class='category-title'>Fashion and Style</div>
                </div>
            </div>

            <div class='col-12 col-sm-6 col-md-4 col-lg-3'>
                <div class='category-card' onclick='onCategoryClick(""Electronics"")'>
                    <img src='https://images.unsplash.com/photo-1498049794561-7780e7231661?auto=format&fit=crop&w=400' alt='Electronics'>
                    <div class='category-title'>Electronics</div>
                </div>
            </div>
        </div>
    </div>

    <div class='bottom-bar'>
        <div>
            <div class='text-muted small'>Total due</div>
            <div class='fw-bold fs-4'>1,500 EGP</div>
        </div>
        <div class='d-flex align-items-center gap-3'>
            <div class='text-end'>
                <div class='small'>Shopping basket: <span class='text-primary fw-bold'>3 items</span></div>
                <div class='text-muted' style='font-size: 0.7rem;'>A new element has been added</div>
            </div>
            <div class='cart-badge fs-3'>
                <i class='bi bi-cart3'></i>
                <span class='badge-count'>3</span>
            </div>
            <button class='btn-pay ms-3' onclick='onPay()'>
                <i class='bi bi-cart-check me-2'></i> Pay Offer
            </button>
        </div>
    </div>

    <script>
        function onCategoryClick(category) {
            window.chrome.webview.postMessage({
                type: 'SELECT_CATEGORY',
                payload: category
            });
        }

        function onPay() {
            window.chrome.webview.postMessage({
                type: 'PAY_ACTION',
                amount: '1,500'
            });
        }
    </script>
</body>
</html>";

            webView.NavigateToString(htmlContent);
        }
        private void OnWebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            string jsonMessage = e.WebMessageAsJson;
            // استخدام JsonDocument لتحليل الرسالة القادمة من الويب
            using (JsonDocument doc = JsonDocument.Parse(jsonMessage))
            {
                string type = doc.RootElement.GetProperty("type").GetString();

                if (type == "SELECT_CATEGORY")
                {
                    string category = doc.RootElement.GetProperty("payload").GetString();
                    MessageBox.Show($"تم اختيار قسم: {category}");
                    // هنا تفتح فورم المنتجات
                }
                else if (type == "PAY_ACTION")
                {
                    string amount = doc.RootElement.GetProperty("amount").GetString();
                    MessageBox.Show($"جاري الانتقال لصفحة الدفع لمبلغ: {amount} EGP");
                }
            }
        }
    }
}
