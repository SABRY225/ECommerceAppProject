using ECommerceApp.Application.Interfaces.Services;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.Text.Json;
using System.Windows.Forms;

namespace ECommerceApp.Presentation.Client
{
    public partial class MyOrdersForm : Form
    {
        private WebView2 webView;
        private readonly IOrderService _orderService;

        public MyOrdersForm(IOrderService orderService)
        {
            InitializeComponent();
            _orderService = orderService;

            this.Text = "E-Comm Suite - Order Tracking";
            this.WindowState = FormWindowState.Maximized;
            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            webView = new WebView2 { Dock = DockStyle.Fill };
            this.Controls.Add(webView);

            await webView.EnsureCoreWebView2Async(null);
            webView.CoreWebView2.WebMessageReceived += OnWebMessageReceived;

            // كود الـ HTML مدمج بالأسفل
            string htmlContent = GetHtmlTemplate();
            webView.NavigateToString(htmlContent);
        }

        private async Task LoadOrders()
        {
            // جلب البيانات من السيرفس
            var orders = await _orderService.GetCustomerOrders(UserSession.CustomerId);

            var data = new
            {
                type = "RENDER_ORDERS",
                orders = orders
            };

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(data, options);
            webView.CoreWebView2.PostWebMessageAsJson(json);
        }
        private void SetCustomerInfo()
        {
            var data = new { type = "SET_CUSTOMER_NAME", name = UserSession.CustomerName };
            string json = JsonSerializer.Serialize(data);
            webView.CoreWebView2.PostWebMessageAsJson(json);
        }
        private async void OnWebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            using JsonDocument doc = JsonDocument.Parse(e.WebMessageAsJson);
            string type = doc.RootElement.GetProperty("type").GetString();

            if (type == "LOAD_ORDERS")
            {
                await LoadOrders();
                SetCustomerInfo();
            }
            // إضافة حدث الرجوع هنا
            else if (type == "BACK_CLICKED")
            {
                this.Close(); 
            }
        }

        private string GetHtmlTemplate()
        {
            return @"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css' rel='stylesheet'>
    <link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.0/font/bootstrap-icons.css'>
    <style>
        :root { --dark-blue: #1e3a58; --bg-gray: #f4f7f9; --success-green: #28a745; }
        body { background-color: var(--bg-gray); font-family: 'Segoe UI', sans-serif; overflow-x: hidden; }
        .navbar { background: white; border-bottom: 1px solid #eee; padding: 10px 40px; }
        .main-container { padding: 40px; display: flex; gap: 25px; }
        
        /* Sidebar List */
        .recent-orders-side { flex: 1; min-width: 300px; }
        .order-list-card { background: white; border-radius: 12px; padding: 15px; margin-bottom: 12px; 
                           border: 2px solid transparent; cursor: pointer; transition: 0.2s; }
        .order-list-card:hover { transform: translateY(-2px); box-shadow: 0 4px 8px rgba(0,0,0,0.05); }
        .order-list-card.active { border-color: var(--dark-blue); background: #f8fbff; }
        
        /* Status Pills */
        .status-pill { font-size: 0.7rem; padding: 3px 10px; border-radius: 20px; font-weight: bold; }
        .status-pending { background: #fff3cd; color: #856404; }
        .status-processing { background: #e0e7ff; color: #4338ca; }
        .status-shipped { background: #e7f3ff; color: #007bff; }
        .status-completed { background: #d4edda; color: #155724; }
        .status-cancelled { background: #f8d7da; color: #721c24; }

        /* Tracking & Items */
        .tracking-section { flex: 3; }
        .card-custom { background: white; border-radius: 12px; padding: 25px; margin-bottom: 25px; border: none; }
        
        .item-row { display: flex; align-items: center; padding: 12px 0; border-bottom: 1px solid #f5f5f5; }
        .item-img { width: 50px; height: 50px; border-radius: 8px; object-fit: cover; margin-right: 15px; background: #eee; }

        /* Summary Card */
        .summary-side { flex: 1.2; }
        .summary-dark-card { background: var(--dark-blue); color: white; border-radius: 12px; padding: 25px; }
        
        .empty-state { text-align: center; padding: 50px; color: #888; }
        
        .back-btn { 
    cursor: pointer; 
    transition: 0.3s; 
    padding: 5px 10px; 
    border-radius: 8px; 
}
.back-btn:hover { 
    background-color: #f0f2f5; 
    color: var(--dark-blue); 
}
    </style>
</head>
<body>
<nav class='navbar d-flex justify-content-between align-items-center shadow-sm'>
    <div class='d-flex align-items-center gap-3'>
        <div class='back-btn' onclick='goBack()'>
            <i class='bi bi-arrow-left fs-4'></i>
        </div>
        <div class='fw-bold'><i class='bi bi-shop me-2'></i> E-Comm Suite</div>
    </div>
    <div class='d-flex gap-3 align-items-center'>
        <span class='fw-bold' id='customerName' style='color: var(--dark-blue);'>Guest User</span>
        <div class='bg-light rounded-circle p-2'><i class='bi bi-person'></i></div>
    </div>
</nav>

    <div class='main-container'>
        <div class='recent-orders-side'>
            <h6 class='fw-bold mb-3'>Recent Orders</h6>
            <div id='orders-list-container'>
                <div class='text-center mt-5'><div class='spinner-border spinner-border-sm text-primary'></div></div>
            </div>
        </div>

        <div class='tracking-section' id='main-detail-view' style='display:none;'>
            <div class='card-custom shadow-sm'>
                <h4 class='fw-bold' id='display-order-id'>Order Details</h4>
                <div class='text-muted small mb-4' id='display-order-date'>-</div>
                
                <div class='d-flex justify-content-between align-items-center p-3 bg-light rounded'>
                    <span class='fw-bold'>Current Status:</span>
                    <span id='display-order-status' class='status-pill status-shipped'>-</span>
                </div>
            </div>

            <div class='card-custom shadow-sm'>
                <h6 class='fw-bold mb-4' id='items-count'>Items (0)</h6>
                <div id='items-list-container'></div>
            </div>
        </div>

        <div class='summary-side' id='summary-view' style='display:none;'>
            <div class='summary-dark-card shadow-sm'>
                <h6 class='mb-4'>Order Summary</h6>
                <div class='d-flex justify-content-between small mb-4'>
                    <span>Total Amount</span>
                    <span class='fs-5 fw-bold' id='display-total-price'>0.00 EGP</span>
                </div>
                <hr style='border-color: rgba(255,255,255,0.2)'>
            </div>
        </div>

        <div id='empty-state' class='flex-grow-1 empty-state'>
            <i class='bi bi-box-seam' style='font-size: 3rem;'></i>
            <p class='mt-3'>Select an order to view details</p>
        </div>
    </div>

<script>
const stateMap = {
            '1': 'Pending',
            '2': 'Processing',
            '3': 'Shipped',
            '4': 'Completed',
            '5': 'Cancelled'
        };

    window.chrome.webview.addEventListener('message', event => {
        const data = event.data;
        if (data.type === 'RENDER_ORDERS') {
            renderOrders(data.orders);
        }
        if (data.type === 'SET_CUSTOMER_NAME') {
        document.getElementById('customerName').innerText = data.name;
        }
    });

    function renderOrders(orders) {
        const container = document.getElementById('orders-list-container');
        if (!orders || orders.length === 0) {
            container.innerHTML = '<p class=""text-muted small"">No orders found.</p>';
            return;
        }
        container.innerHTML = '';
        orders.forEach(order => {
            const statusText = stateMap[String(order.state)] || 'Unknown';
            const card = document.createElement('div');
            card.className = 'order-list-card shadow-sm';
            card.id = 'card-' + order.id;
            card.innerHTML = `
                <div class='d-flex justify-content-between mb-1'>
                    <span class='fw-bold small'>#ORD-${order.id}</span>
                    <span class='status-pill status-${statusText.toLowerCase()}'>${statusText}</span>
                </div>
                <div class='text-muted' style='font-size: 0.75rem;'>${formatDate(order.orderDate)}</div>
                <div class='text-muted small' style='font-size: 0.7rem;'>Items: ${order.productsCount}</div>
            `;
            card.onclick = () => selectOrder(order);
            container.appendChild(card);
        });
    }

    function selectOrder(order) {
        document.getElementById('empty-state').style.display = 'none';
        document.getElementById('main-detail-view').style.display = 'block';
        document.getElementById('summary-view').style.display = 'block';

        document.querySelectorAll('.order-list-card').forEach(c => c.classList.remove('active'));
        document.getElementById('card-' + order.id).classList.add('active');

        document.getElementById('display-order-id').innerText = 'Order Details #ORD-' + order.id;
        document.getElementById('display-order-date').innerText = 'Order Date: ' + formatDate(order.orderDate);
        const stateKey = String(order.state);
        const statusText = stateMap[stateKey] || 'Unknown';
        document.getElementById('display-order-status').innerText = statusText;
        document.getElementById('display-order-status').className = 'status-pill status-' + statusText.toLowerCase();
        document.getElementById('display-total-price').innerText = order.totalAmount.toLocaleString() + ' EGP';

        const itemsContainer = document.getElementById('items-list-container');

        document.getElementById('items-count').innerText = `Items (${order.productsCount})`;
        itemsContainer.innerHTML = '';
        
        if (order.orderProducts) {
            order.orderProducts.forEach(item => {
                itemsContainer.innerHTML += `
                    <div class='item-row'>
                        <img src='${item.imagePath}' class='item-img'>
                        <div class='flex-grow-1'>
                            <div class='fw-bold small'>${item.productName || 'Product'}</div>
                            <div class='text-muted small'>Qty: ${item.quantity}</div>
                        </div>
                        <div class='fw-bold small'>${item.priceAtPurchase} EGP</div>
                    </div>`;
            });
        }
    }

    function getStatusClass(state) {
        if (!state) return 'status-pending';
        const s = state.toLowerCase();
        if (s.includes('deliver') || s.includes('تم التوصيل')) return 'status-delivered';
        if (s.includes('ship') || s.includes('way') || s.includes('شحن')) return 'status-shipped';
        return 'status-pending'; // للـ Pending أو Processing
    }

    function formatDate(dateStr) {
        const date = new Date(dateStr);
        return date.toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' });
    }
function goBack() {
    window.chrome.webview.postMessage({ type: 'BACK_CLICKED' });
}
    window.onload = () => {
        window.chrome.webview.postMessage({ type: 'LOAD_ORDERS' });
    };
</script>
</body>
</html>";
        }
    }
}