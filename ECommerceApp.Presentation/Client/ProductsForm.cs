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
    public partial class ProductsForm : Form
    {
        private WebView2 webView;
        public ProductsForm(string categoryName = "Electronics")
        {
            InitializeComponent();
            this.Text = $"E-Comm Suite - {categoryName}";
            this.WindowState = FormWindowState.Maximized;
            InitializeWebView(categoryName);
        }
        private async void InitializeWebView(string categoryName)
        {
            webView = new WebView2();
            webView.Dock = DockStyle.Fill;
            this.Controls.Add(webView);

            await webView.EnsureCoreWebView2Async(null);
            webView.CoreWebView2.WebMessageReceived += OnWebMessageReceived;

            // كود HTML/CSS/JS مدمج
            string htmlContent = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css' rel='stylesheet'>
    <link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.0/font/bootstrap-icons.css'>
    <style>
        :root {{ --dark-blue: #1e3a58; --light-bg: #f4f6f9; }}
        body {{ background-color: var(--light-bg); font-family: 'Segoe UI', sans-serif; padding-bottom: 100px; }}
        
        /* Navbar */
        .navbar {{ background: white; border-bottom: 1px solid #eee; padding: 12px 30px; }}
        
        /* Breadcrumb & Search */
        .breadcrumb-section {{ padding: 20px 40px; font-size: 0.85rem; color: #666; }}
        .search-container {{ padding: 0 40px 20px 40px; }}
        .search-input {{ 
            width: 100%; padding: 12px 20px; border-radius: 10px; 
            border: 1px solid #ddd; background: white; outline: none;
        }}

        /* Product Cards */
        .product-card {{ 
            background: white; border-radius: 15px; overflow: hidden; 
            border: none; box-shadow: 0 4px 10px rgba(0,0,0,0.03); transition: 0.3s;
        }}
        .product-card:hover {{ transform: translateY(-5px); box-shadow: 0 8px 20px rgba(0,0,0,0.08); }}
        .product-img-wrapper {{ background: #000; height: 250px; display: flex; align-items: center; justify-content: center; }}
        .product-img {{ max-width: 100%; max-height: 100%; object-fit: contain; }}
        .product-info {{ padding: 20px; }}
        .product-title {{ font-size: 0.95rem; color: #333; margin-bottom: 8px; font-weight: 500; }}
        .product-price {{ color: var(--dark-blue); font-weight: bold; font-size: 1.1rem; }}
        .btn-add-cart {{ 
            width: 100%; background: var(--dark-blue); color: white; 
            border: none; padding: 10px; border-radius: 8px; margin-top: 15px;
            display: flex; align-items: center; justify-content: center; gap: 8px;
        }}

        /* Footer Bar */
        .bottom-bar {{ 
            position: fixed; bottom: 0; left: 0; right: 0; background: white; 
            padding: 15px 40px; border-top: 1px solid #eee; display: flex; 
            justify-content: space-between; align-items: center; z-index: 1000;
        }}
        .btn-pay {{ background: var(--dark-blue); color: white; padding: 10px 35px; border-radius: 8px; border: none; }}
    </style>
</head>
<body>

    <nav class='navbar d-flex justify-content-between align-items-center shadow-sm'>
        <div class='d-flex align-items-center gap-2'>
            <div class='bg-dark text-white p-2 rounded'><i class='bi bi-shop'></i></div>
            <span class='fw-bold'>E-Comm Suite</span>
        </div>
        <div class='d-flex gap-4 align-items-center'>
            <span class='small fw-bold'>My Orders</span>
            <div class='bg-light p-2 rounded'><i class='bi bi-box'></i></div>
            <div class='d-flex align-items-center gap-2'>
                <span class='small fw-bold'>Ahmed Sabry</span>
                <div class='bg-light rounded-circle p-2'><i class='bi bi-person'></i></div>
            </div>
        </div>
    </nav>

    <div class='breadcrumb-section'>
        Categories &nbsp; <i class='bi bi-chevron-right small'></i> &nbsp; <b>{categoryName}</b>
    </div>

    <div class='search-container'>
        <input type='text' class='search-input' placeholder='Find the product you want...'>
    </div>

    <div class='container-fluid px-5'>
        <div class='row g-4' id='product-list'>
            {GenerateProductCards()}
        </div>
    </div>

    <div class='bottom-bar shadow'>
        <div class='d-flex align-items-center gap-4'>
            <div>
                <div class='text-muted small'>Total due</div>
                <div class='fw-bold fs-5'>1,500 EGP</div>
            </div>
        </div>
        <div class='d-flex align-items-center gap-4'>
            <div class='text-end'>
                <div class='small fw-bold'>Shopping basket: <span class='text-primary'>3 items</span></div>
                <div class='text-muted small'>A new element has been added</div>
            </div>
            <div class='fs-3'><i class='bi bi-cart3'></i></div>
            <button class='btn-pay' onclick='onPay()'><i class='bi bi-cart-check me-2'></i> Pay Offer</button>
        </div>
    </div>

    <script>
        function addToCart(productId) {{
            window.chrome.webview.postMessage({{
                type: 'ADD_TO_CART',
                id: productId
            }});
        }}

        function onPay() {{
            window.chrome.webview.postMessage({{ type: 'PAY_ACTION' }});
        }}
    </script>
</body>
</html>";

            webView.NavigateToString(htmlContent);
        }

        private string GenerateProductCards()
        {
            string cards = "";
            for (int i = 0; i < 6; i++)
            {
                cards += @"
                <div class='col-12 col-sm-6 col-md-4'>
                    <div class='product-card'>
                        <div class='product-img-wrapper'>
                            <img src='https://cdn.dxomark.com/wp-content/uploads/medias/post-155689/Apple-iPhone-15-Pro-Max_blue-titanium_featured-image.jpg' class='product-img'>
                        </div>
                        <div class='product-info text-center'>
                            <div class='product-title'>Go - Natural Titanium 15 iPhone</div>
                            <div class='product-price'>10,000 EGP</div>
                            <button class='btn-add-cart' onclick='addToCart(" + i + @")'>
                                <i class='bi bi-cart-plus'></i> Add To Cart
                            </button>
                        </div>
                    </div>
                </div>";
            }
            return cards;
        }

        private void OnWebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            using (JsonDocument doc = JsonDocument.Parse(e.WebMessageAsJson))
            {
                string type = doc.RootElement.GetProperty("type").GetString();
                if (type == "ADD_TO_CART")
                {
                    int id = doc.RootElement.GetProperty("id").GetInt32();
                    MessageBox.Show($"تم إضافة المنتج رقم {id} إلى السلة بنجاح!");
                }
            }
        }
    }
}
