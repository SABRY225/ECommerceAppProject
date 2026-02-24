using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;
using System.Text.Json;

namespace ECommerceApp.Presentation.Client
{
    public partial class CartForm : Form
    {
        private WebView2 webView;
        public CartForm()
        {
            InitializeComponent();
            this.Text = "E-Comm Suite - Shopping Cart";
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
        body { background-color: var(--bg-gray); font-family: 'Segoe UI', sans-serif; }
        
        .navbar { background: white; border-bottom: 1px solid #eee; padding: 10px 40px; }
        .cart-container { padding: 40px; display: flex; gap: 30px; }
        
        /* Items List */
        .cart-items-section { flex: 2; }
        .cart-item-card { 
            background: white; border-radius: 12px; padding: 20px; 
            display: flex; align-items: center; gap: 20px; margin-bottom: 15px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.02);
        }
        .item-img { width: 100px; height: 100px; object-fit: cover; border-radius: 8px; background: #eee; }
        .item-details { flex: 1; }
        .item-title { font-weight: bold; font-size: 1.1rem; color: #333; }
        .item-meta { font-size: 0.85rem; color: #888; margin-bottom: 10px; }
        .item-price { font-weight: 800; color: #333; }

        /* Order Summary */
        .summary-card { 
            flex: 1; background: white; border-radius: 12px; padding: 25px; 
            height: fit-content; box-shadow: 0 2px 10px rgba(0,0,0,0.03);
        }
        .summary-row { display: flex; justify-content: space-between; margin-bottom: 15px; color: #666; }
        .total-row { border-top: 1px dashed #ddd; padding-top: 15px; margin-top: 15px; font-weight: bold; font-size: 1.3rem; }
        
        .btn-checkout { 
            width: 100%; background: var(--dark-blue); color: white; border: none; 
            padding: 15px; border-radius: 8px; font-weight: bold; margin-top: 20px; 
        }

        /* Controls */
        .qty-control { display: flex; align-items: center; gap: 10px; border: 1px solid #eee; border-radius: 6px; padding: 2px 10px; }
        .btn-remove { color: #dc3545; border: none; background: none; font-size: 0.85rem; cursor: pointer; }
    </style>
</head>
<body>

    <nav class='navbar d-flex justify-content-between align-items-center'>
        <div class='fw-bold'><i class='bi bi-shop me-2'></i> E-Comm Suite</div>
        <div class='d-flex gap-3 align-items-center'>
            <span class='small fw-bold'>Ahmed Sabry</span>
            <div class='bg-light rounded-circle p-2'><i class='bi bi-person'></i></div>
        </div>
    </nav>

    <div class='container cart-container'>
        <div class='cart-items-section'>
            <h3 class='mb-4 fw-bold'>Shopping Cart <small class='text-muted fs-6'>(3 items)</small></h3>
            
            <div class='cart-item-card'>
                <img src='https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?w=200' class='item-img'>
                <div class='item-details'>
                    <div class='item-title'>Smartphone Pro Max</div>
                    <div class='item-meta'>Color: Stellar Black | Capacity: 256 GB</div>
                    <div class='item-price'>2,999 EGP</div>
                </div>
                <div class='text-end'>
                    <div class='qty-control mb-2'>
                        <button class='border-0 bg-transparent'>-</button>
                        <span>1</span>
                        <button class='border-0 bg-transparent'>+</button>
                    </div>
                    <button class='btn-remove' onclick='removeItem(1)'><i class='bi bi-trash me-1'></i> Remove</button>
                </div>
            </div>

            <div class='cart-item-card'>
                <img src='https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=200' class='item-img'>
                <div class='item-details'>
                    <div class='item-title'>Wireless Headphones</div>
                    <div class='item-meta'>Color: Pearl White | Noise Cancelling</div>
                    <div class='item-price'>850 EGP</div>
                </div>
                <div class='text-end'>
                    <div class='qty-control mb-2'>
                        <button class='border-0 bg-transparent'>-</button>
                        <span>1</span>
                        <button class='border-0 bg-transparent'>+</button>
                    </div>
                    <button class='btn-remove' onclick='removeItem(2)'><i class='bi bi-trash me-1'></i> Remove</button>
                </div>
            </div>
        </div>

        <div class='summary-card'>
            <h5 class='fw-bold mb-4'>Order Summary</h5>
            <div class='summary-row'><span>Subtotal</span><span>4,299 EGP</span></div>
            <div class='summary-row'><span>Shipping</span><span class='text-success fw-bold'>Free</span></div>
            <div class='summary-row'><span>Tax (15%)</span><span>644.85 EGP</span></div>
            
            <div class='total-row d-flex justify-content-between text-dark'>
                <span>Total</span>
                <span>4,943.85 EGP</span>
            </div>
            <div class='text-muted small text-end'>Including VAT</div>

            <button class='btn-checkout' onclick='checkout()'>Checkout</button>
        </div>
    </div>

    <script>
        function removeItem(id) {
            window.chrome.webview.postMessage({ action: 'REMOVE', itemId: id });
        }
        function checkout() {
            window.chrome.webview.postMessage({ action: 'CHECKOUT' });
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
                if (action == "REMOVE")
                {
                    int id = doc.RootElement.GetProperty("itemId").GetInt32();
                    MessageBox.Show($"جاري حذف المنتج رقم {id} من السلة...");
                }
                else if (action == "CHECKOUT")
                {
                    MessageBox.Show("جاري الانتقال لصفحة الدفع النهائية...");
                }
            }
        }
    }
}
