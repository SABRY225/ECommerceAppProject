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
    public partial class CheckoutForm : Form
    {
        private WebView2 webView;
        public CheckoutForm()
        {
            InitializeComponent();
            this.Text = "E-Comm Suite - Checkout";
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
        :root { --dark-blue: #1e3a58; --bg-gray: #f4f7f9; }
        body { background-color: var(--bg-gray); font-family: 'Segoe UI', sans-serif; padding-top: 20px; }
        
        .navbar { background: white; border-bottom: 1px solid #eee; padding: 10px 40px; position: fixed; top: 0; width: 100%; z-index: 1000; }
        .main-content { margin-top: 80px; padding: 40px; display: flex; gap: 30px; }
        
        /* Containers */
        .checkout-section { flex: 2; }
        .card-custom { background: white; border-radius: 12px; padding: 30px; margin-bottom: 25px; border: none; box-shadow: 0 4px 12px rgba(0,0,0,0.03); }
        .section-title { font-weight: bold; font-size: 1.2rem; color: var(--dark-blue); margin-bottom: 25px; display: flex; align-items: center; gap: 10px; }

        /* Form Controls */
        .form-label { font-size: 0.85rem; font-weight: 600; color: #555; }
        .form-control { background: #f9f9f9; border: 1px solid #eee; padding: 12px; border-radius: 8px; }
        .form-control:focus { background: white; box-shadow: none; border-color: var(--dark-blue); }

        /* Sidebar Summary */
        .summary-side { flex: 1; position: sticky; top: 100px; height: fit-content; }
        .order-summary-card { background: white; border-radius: 12px; padding: 25px; box-shadow: 0 4px 12px rgba(0,0,0,0.03); }
        .total-box { background: #fcfcfc; border-top: 1px solid #eee; padding-top: 20px; margin-top: 20px; }
        .btn-confirm { width: 100%; background: var(--dark-blue); color: white; border: none; padding: 15px; border-radius: 8px; font-weight: bold; font-size: 1.1rem; transition: 0.3s; }
        .btn-confirm:hover { background: #162c43; transform: translateY(-2px); }

        /* Payment Method */
        .payment-option { border: 2px solid #eee; border-radius: 10px; padding: 15px; display: flex; align-items: center; gap: 15px; cursor: pointer; transition: 0.2s; }
        .payment-option.active { border-color: var(--dark-blue); background: #f0f4f8; }

        /* Info Badges */
        .info-badge { display: flex; align-items: center; gap: 8px; font-size: 0.75rem; color: #777; background: white; padding: 10px; border-radius: 8px; flex: 1; justify-content: center; }
    </style>
</head>
<body>

    <nav class='navbar d-flex justify-content-between align-items-center shadow-sm'>
        <div class='fw-bold fs-5'><i class='bi bi-shop me-2'></i> E-Comm Suite</div>
        <div class='d-flex gap-3 align-items-center'>
            <span class='small fw-bold'>Ahmed Sabry</span>
            <div class='bg-light rounded-circle p-2'><i class='bi bi-person'></i></div>
        </div>
    </nav>

    <div class='container main-content'>
        <div class='checkout-section'>
            <h2 class='fw-bold mb-4'>Checkout</h2>
            <p class='text-muted mb-4'>Please review your order and enter shipping details to complete the process</p>

            <div class='card-custom'>
                <div class='section-title'><i class='bi bi-truck'></i> Shipping Information</div>
                <div class='row g-3'>
                    <div class='col-12'>
                        <label class='form-label'>Full Name</label>
                        <input type='text' id='fullName' class='form-control' placeholder='Enter your full name'>
                    </div>
                    <div class='col-md-6'>
                        <label class='form-label'>Phone Number</label>
                        <input type='text' id='phone' class='form-control' placeholder='05xxxxxxxx'>
                    </div>
                    <div class='col-md-6'>
                        <label class='form-label'>WhatsApp Number (Optional)</label>
                        <input type='text' id='whatsapp' class='form-control' placeholder='05xxxxxxxx'>
                    </div>
                    <div class='col-12'>
                        <label class='form-label'>Detailed Address</label>
                        <textarea id='address' class='form-control' rows='3' placeholder='City, District, Street Name, Building No.'></textarea>
                    </div>
                </div>
            </div>

            <div class='card-custom'>
                <div class='section-title'><i class='bi bi-credit-card'></i> Payment Method</div>
                <div class='payment-option active'>
                    <input type='radio' checked name='payType'>
                    <div>
                        <div class='fw-bold'>Cash on Delivery</div>
                        <div class='small text-muted'>Pay in cash when you receive your order from the delivery representative</div>
                    </div>
                </div>
            </div>
        </div>

        <div class='summary-side'>
            <div class='order-summary-card'>
                <div class='section-title mb-4'><i class='bi bi-cart-check'></i> Order Summary</div>
                <div class='d-flex justify-content-between mb-4'>
                    <span class='fw-bold fs-5'>Total</span>
                    <span class='fw-bold fs-4' style='color:var(--dark-blue)'>4,943.85 EGP</span>
                </div>
                <button class='btn-confirm' onclick='confirmOrder()'>Confirm Order <i class='bi bi-check-circle-fill ms-2'></i></button>
            </div>

            <div class='d-flex gap-2 mt-3'>
                <div class='info-badge shadow-sm'><i class='bi bi-patch-check'></i> Guaranteed Quality</div>
                <div class='info-badge shadow-sm'><i class='bi bi-arrow-repeat'></i> Easy Returns</div>
            </div>
        </div>
    </div>

    <script>
        function confirmOrder() {
            const data = {
                action: 'CONFIRM',
                name: document.getElementById('fullName').value,
                address: document.getElementById('address').value,
                phone: document.getElementById('phone').value
            };
            window.chrome.webview.postMessage(data);
        }
    </script>
</body>
</html>";

            webView.NavigateToString(htmlContent);
        }
        private void OnWebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            string json = e.WebMessageAsJson;
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                var root = doc.RootElement;
                if (root.GetProperty("action").GetString() == "CONFIRM")
                {
                    string name = root.GetProperty("name").GetString();
                    if (string.IsNullOrEmpty(name))
                    {
                        MessageBox.Show("Please enter your name!", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    MessageBox.Show($"Order Confirmed Successfully for {name}!\nThank you for shopping with E-Comm Suite.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close(); // إغلاق الفورم بعد النجاح
                }
            }
        }
    }
}
