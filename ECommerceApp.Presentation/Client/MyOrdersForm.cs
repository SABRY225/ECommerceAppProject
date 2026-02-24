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

namespace ECommerceApp.Presentation.Client
{
    public partial class MyOrdersForm : Form
    {
        private WebView2 webView;
        public MyOrdersForm()
        {
            InitializeComponent();
            this.Text = "E-Comm Suite - Order Tracking";
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
    <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css' rel='stylesheet'>
    <link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.0/font/bootstrap-icons.css'>
    <style>
        :root { --dark-blue: #1e3a58; --bg-gray: #f4f7f9; --success-green: #28a745; }
        body { background-color: var(--bg-gray); font-family: 'Segoe UI', sans-serif; }
        
        .navbar { background: white; border-bottom: 1px solid #eee; padding: 10px 40px; }
        .main-container { padding: 40px; display: flex; gap: 25px; }

        /* Left Sidebar: Recent Orders */
        .recent-orders-side { flex: 1; }
        .order-list-card { background: white; border-radius: 12px; padding: 15px; margin-bottom: 12px; border: 2px solid transparent; cursor: pointer; }
        .order-list-card.active { border-color: var(--dark-blue); }
        .status-pill { font-size: 0.7rem; padding: 3px 10px; border-radius: 20px; font-weight: bold; text-transform: uppercase; }
        .status-delivery { background: #e7f3ff; color: #007bff; }

        /* Main Section: Tracking */
        .tracking-section { flex: 3; }
        .card-custom { background: white; border-radius: 12px; padding: 30px; margin-bottom: 25px; border: none; }
        
        /* Stepper Tracking */
        .stepper { display: flex; justify-content: space-between; position: relative; margin-top: 40px; }
        .stepper::before { content: ''; position: absolute; top: 15px; left: 10%; right: 10%; height: 3px; background: #eee; z-index: 1; }
        .step-progress { position: absolute; top: 15px; left: 10%; width: 40%; height: 3px; background: var(--dark-blue); z-index: 2; }
        .step { z-index: 3; text-align: center; }
        .step-icon { width: 35px; height: 35px; background: white; border: 3px solid #eee; border-radius: 50%; display: flex; align-items: center; justify-content: center; margin: 0 auto 10px; transition: 0.3s; }
        .step.active .step-icon { border-color: var(--dark-blue); background: var(--dark-blue); color: white; }
        .step-label { font-size: 0.8rem; font-weight: bold; color: #333; }
        .step-time { font-size: 0.7rem; color: #888; }

        /* Items Table */
        .item-row { display: flex; align-items: center; padding: 15px 0; border-bottom: 1px solid #f5f5f5; }
        .item-img { width: 60px; height: 60px; border-radius: 8px; object-fit: cover; margin-right: 15px; background: #eee; }
        
        /* Summary & Address */
        .summary-side { flex: 1.2; }
        .address-box { background: white; border-radius: 12px; padding: 25px; margin-bottom: 20px; }
        .summary-dark-card { background: var(--dark-blue); color: white; border-radius: 12px; padding: 25px; }
    </style>
</head>
<body>

    <nav class='navbar d-flex justify-content-between align-items-center shadow-sm'>
        <div class='fw-bold'><i class='bi bi-shop me-2'></i> E-Comm Suite</div>
        <div class='d-flex gap-3 align-items-center'>
            <span class='small fw-bold'>Ahmed Sabry</span>
            <div class='bg-light rounded-circle p-2'><i class='bi bi-person'></i></div>
        </div>
    </nav>

    <div class='main-container'>
        <div class='recent-orders-side'>
            <div class='d-flex justify-content-between align-items-center mb-3'>
                <h6 class='fw-bold m-0'>Recent Orders</h6>
                <span class='badge bg-light text-dark'>8 Orders</span>
            </div>
            
            <div class='order-list-card active shadow-sm'>
                <div class='d-flex justify-content-between mb-2'>
                    <span class='fw-bold'>#ORD-8829</span>
                    <span class='status-pill status-delivery'>In Delivery</span>
                </div>
                <div class='text-muted small'>October 12, 2023</div>
            </div>

            <div class='order-list-card shadow-sm'>
                <div class='d-flex justify-content-between mb-2'>
                    <span class='fw-bold'>#ORD-8742</span>
                    <span class='status-pill bg-light text-success'>Delivered</span>
                </div>
                <div class='text-muted small'>October 05, 2023</div>
            </div>
        </div>

        <div class='tracking-section'>
            <div class='card-custom shadow-sm'>
                <h4 class='fw-bold'>Order Details #ORD-8829</h4>
                <div class='text-muted small mb-4'>Order Date: Oct 12, 2023</div>

                <div class='stepper'>
                    <div class='step-progress'></div>
                    <div class='step active'>
                        <div class='step-icon'><i class='bi bi-check-lg'></i></div>
                        <div class='step-label'>Ordered</div>
                        <div class='step-time'>2:30 PM</div>
                    </div>
                    <div class='step active'>
                        <div class='step-icon'><i class='bi bi-truck'></i></div>
                        <div class='step-label'>Processing</div>
                        <div class='step-time'>10:00 AM</div>
                    </div>
                    <div class='step'>
                        <div class='step-icon'><i class='bi bi-house-door'></i></div>
                        <div class='step-label'>Shipped</div>
                        <div class='step-time'>--:--</div>
                    </div>
                </div>
            </div>

            <div class='card-custom shadow-sm'>
                <h6 class='fw-bold mb-4'>Items (2)</h6>
                <div class='item-row'>
                    <img src='https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=100' class='item-img'>
                    <div class='flex-grow-1'>
                        <div class='fw-bold small'>Nike Athletic Shoe - Red</div>
                        <div class='text-muted' style='font-size:0.7rem'>Color: Red | Size: 42</div>
                        <div class='small fw-bold'>Qty: 1</div>
                    </div>
                    <div class='fw-bold'>450.00 EGP</div>
                </div>
                <div class='item-row border-0'>
                    <img src='https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=100' class='item-img'>
                    <div class='flex-grow-1'>
                        <div class='fw-bold small'>Classic Wristwatch</div>
                        <div class='text-muted' style='font-size:0.7rem'>Color: Silver | Strap: Leather</div>
                        <div class='small fw-bold'>Qty: 1</div>
                    </div>
                    <div class='fw-bold'>890.00 EGP</div>
                </div>
            </div>
        </div>

        <div class='summary-side'>
            <div class='address-box shadow-sm'>
                <h6 class='fw-bold mb-3'><i class='bi bi-geo-alt me-2'></i>Shipping Address</h6>
                <div class='fw-bold small mb-1'>Ahmed Mohammed Abdullah</div>
                <div class='text-muted small mb-2'>Tahlia St, Al-Murooj District<br>Riyadh, Saudi Arabia</div>
                <div class='small fw-bold text-dark'><i class='bi bi-telephone me-1'></i> +2010 985 838 17</div>
            </div>

            <div class='summary-dark-card shadow-sm'>
                <h6 class='mb-4'>Order Summary</h6>
                <div class='d-flex justify-content-between small mb-2'><span>Subtotal</span><span>1,340.00 EGP</span></div>
                <div class='d-flex justify-content-between small mb-2'><span>Shipping fee</span><span>35.00 EGP</span></div>
                <div class='d-flex justify-content-between small mb-4'><span>Tax (15%)</span><span>201.00 EGP</span></div>
                <hr style='border-color: rgba(255,255,255,0.2)'>
                <div class='d-flex justify-content-between fw-bold fs-5'>
                    <span>Total</span>
                    <span>1,576.00 EGP</span>
                </div>
            </div>
        </div>
    </div>

</body>
</html>";

            webView.NavigateToString(htmlContent);
        }
    }
}
