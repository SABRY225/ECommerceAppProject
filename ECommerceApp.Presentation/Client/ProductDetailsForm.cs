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
    public partial class ProductDetailsForm : Form
    {
        private WebView2 webView;
        public ProductDetailsForm()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
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
        :root { --dark-blue: #1e3a58; --bg-gray: #f8f9fa; }
        body { background-color: white; font-family: 'Segoe UI', sans-serif; overflow-x: hidden; }
        
        /* Navbar */
        .navbar { background: white; border-bottom: 1px solid #eee; padding: 10px 40px; }
        .breadcrumb-nav { padding: 15px 40px; font-size: 0.8rem; color: #888; }

        /* Product Section */
        .product-container { padding: 40px; display: flex; gap: 50px; }
        .image-box { 
            flex: 1; background: #000; border-radius: 15px; 
            display: flex; align-items: center; justify-content: center; height: 500px;
        }
        .image-box img { max-width: 90%; max-height: 90%; object-fit: contain; }
        
        .details-box { flex: 1.2; }
        .status-badge { color: #28a745; font-weight: 600; margin-bottom: 15px; display: block; }
        .product-title { font-size: 2.2rem; fw-bold; color: var(--dark-blue); margin-bottom: 20px; }
        .product-price { font-size: 2rem; font-weight: 800; color: #333; margin-bottom: 25px; }
        .description { color: #6c757d; line-height: 1.8; margin-bottom: 30px; font-size: 1.05rem; }

        /* Controls */
        .action-row { display: flex; gap: 15px; margin-bottom: 40px; }
        .btn-add { background: var(--dark-blue); color: white; padding: 12px 40px; border-radius: 8px; flex: 2; border: none; font-weight: 600; }
        .counter-box { 
            display: flex; align-items: center; border: 1px solid #ddd; 
            border-radius: 8px; padding: 5px 15px; flex: 1; justify-content: space-between;
        }
        .counter-btn { border: none; background: none; font-size: 1.2rem; cursor: pointer; color: #555; }

        /* Feature Cards */
        .feature-card { 
            background: white; border: 1px solid #f0f0f0; border-radius: 12px; 
            padding: 20px; display: flex; align-items: center; gap: 15px; flex: 1;
        }
        .feature-icon { font-size: 1.5rem; color: var(--dark-blue); }

        /* Footer Bar */
        .bottom-bar { 
            position: fixed; bottom: 0; left: 0; right: 0; background: white; 
            padding: 15px 40px; border-top: 1px solid #eee; display: flex; 
            justify-content: space-between; align-items: center; box-shadow: 0 -5px 15px rgba(0,0,0,0.02);
        }
    </style>
</head>
<body>

    <nav class='navbar d-flex justify-content-between align-items-center'>
        <div class='fw-bold fs-5'><i class='bi bi-shop me-2'></i> E-Comm Suite</div>
        <div class='d-flex gap-4 align-items-center small fw-bold'>
            <span>My Orders</span> <div class='bg-light p-2 rounded'><i class='bi bi-person'></i></div> <span>Ahmed Sabry</span>
        </div>
    </nav>

    <div class='breadcrumb-nav'>
        Categories > Electronics > <span class='text-dark fw-bold'>Ultra Pro Max Phone - 2024 Edition 512GB</span>
    </div>

    <div class='product-container'>
        <div class='image-box'>
            <img src='https://cdn.dxomark.com/wp-content/uploads/medias/post-155689/Apple-iPhone-15-Pro-Max_blue-titanium_featured-image.jpg'>
        </div>

        <div class='details-box'>
            <span class='status-badge'><i class='bi bi-check-circle-fill me-2'></i>Available in stock</span>
            <h1 class='product-title'>Ultra Pro Max Phone - 2024 Edition 512GB</h1>
            <div class='product-price'>50,000 EGP</div>
            <p class='description'>
                Introducing the world's most powerful smartphone, featuring a 200MP cinematic camera and a blazing-fast processor for an unparalleled gaming experience. It boasts a stunning 120Hz AMOLED display.
            </p>

            <div class='action-row'>
                <button class='btn-add' onclick='addToCart()'><i class='bi bi-cart3 me-2'></i> Add to cart</button>
                <div class='counter-box'>
                    <button class='counter-btn' onclick='updateQty(-1)'>-</button>
                    <span id='qty' class='fw-bold'>1</span>
                    <button class='counter-btn' onclick='updateQty(1)'>+</button>
                </div>
            </div>

            <div class='d-flex gap-3'>
                <div class='feature-card shadow-sm'>
                    <i class='bi bi-truck feature-icon'></i>
                    <div><b class='small d-block'>Fast shipping</b><small class='text-muted'>Delivery within 2-3 days</small></div>
                </div>
                <div class='feature-card shadow-sm'>
                    <i class='bi bi-shield-check feature-icon'></i>
                    <div><b class='small d-block'>Two-year warranty</b><small class='text-muted'>Authorized Agent Guarantee</small></div>
                </div>
            </div>
        </div>
    </div>

    <div class='bottom-bar'>
        <div><small class='text-muted d-block'>Total due</small><b class='fs-5'>1,500 EGP</b></div>
        <div class='d-flex align-items-center gap-4'>
            <div class='text-end small'><span class='text-primary fw-bold'>3 items</span> in basket</div>
            <button class='btn btn-dark py-2 px-4' style='background:#1e3a58' onclick='pay()'><i class='bi bi-cart-check me-2'></i> Pay Offer</button>
        </div>
    </div>

    <script>
        let count = 1;
        function updateQty(val) {
            count = Math.max(1, count + val);
            document.getElementById('qty').innerText = count;
        }
        function addToCart() {
            window.chrome.webview.postMessage({ action: 'ADD', quantity: count });
        }
        function pay() {
            window.chrome.webview.postMessage({ action: 'PAY' });
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
                if (action == "ADD")
                {
                    int qty = doc.RootElement.GetProperty("quantity").GetInt32();
                    MessageBox.Show($"تم إضافة عدد ({qty}) من المنتج للسلة!");
                }
                else if (action == "PAY")
                {
                    MessageBox.Show("الانتقال لعملية الدفع...");
                }
            }
        }
    }
}
