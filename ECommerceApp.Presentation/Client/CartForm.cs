using ECommerceApp.Application.Interfaces.Services;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.Text.Json;

namespace ECommerceApp.Presentation.Client
{
    public partial class CartForm : Form
    {
        private WebView2 webView;
        private readonly IOrderService _OrderService;
        private readonly ICartService _cartService;
        public CartForm(ICartService cartService, IOrderService orderService)
        {
            InitializeComponent();
            _OrderService = orderService;
            _cartService = cartService;
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
            webView.CoreWebView2.DOMContentLoaded += async (s, e) => {
                await LoadCartData();
            };
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
.qty-control { 
    display: flex; 
    align-items: center; 
    gap: 15px; 
    border: 1px solid #ddd; 
    border-radius: 8px; 
    padding: 5px 12px; 
    background: #fff;
}
.qty-control button { 
    font-weight: bold; 
    font-size: 1.2rem; 
    cursor: pointer; 
    color: var(--dark-blue);
    transition: 0.2s;
}
.qty-control button:hover:not(:disabled) { 
    color: #0d6efd; 
}
.qty-control button:disabled {
    color: #ccc;
    cursor: not-allowed;
}
.btn-back {
    border: 1px solid #ddd;
    background: white;
    padding: 5px 15px;
    border-radius: 8px;
    cursor: pointer;
    transition: 0.2s;
    display: flex;
    align-items: center;
    gap: 5px;
}
.btn-back:hover {
    background: #f0f0f0;
    border-color: #bbb;
}
    </style>
</head>
<body>
<nav class='navbar d-flex justify-content-between align-items-center'>
    <div class='d-flex align-items-center gap-3'>
        <button class='btn-back' onclick='goBack()'>
            <i class='bi bi-arrow-left'></i> Back
        </button>
        <div class='fw-bold'><i class='bi bi-shop me-2'></i> E-Comm Suite</div>
    </div>
    <div class='d-flex gap-3 align-items-center'>
        <span class='small fw-bold' id=""clientName"">Loading...</span>
        <div class='bg-light rounded-circle p-2'><i class='bi bi-person'></i></div>
    </div>
</nav>

    <div class='container cart-container'>
        <div class='cart-items-section' id=""itemsList"">
            </div>

        <div class='summary-card' id=""summaryBox"">
            </div>
    </div>

    <script>
        window.chrome.webview.addEventListener('message', event => {
            const data = event.data;
            if (data.type === 'RENDER_CART') {
                renderCart(data);
            }
        });

        function renderCart(data) {
            document.getElementById('clientName').innerText = data.customerName;
            
            // 1. عرض المنتجات
            const itemsContainer = document.getElementById('itemsList');
            let itemsHtml = `<h3 class='mb-4 fw-bold'>Shopping Cart <small class='text-muted fs-6'>(${data.items.length} items)</small></h3>`;
            
            if (data.items.length === 0) {
                itemsHtml += ""<div class='alert alert-info'>Your cart is empty</div>"";
            }

            data.items.forEach(item => {
    itemsHtml += `
    <div class='cart-item-card'>
        <img src='${item.imagePath }' class='item-img'>
        <div class='item-details'>
            <div class='item-title'>${item.productName}</div>
            <div class='item-price'>${item.price.toLocaleString()} EGP</div>
        </div>
        <div class='text-end'>
            <div class='qty-control mb-2'>
                <button class='border-0 bg-transparent' onclick='updateQty(${item.productId}, ${item.quantity - 1})' ${item.quantity <= 1 ? 'disabled' : ''}>-</button>
                <span class='fw-bold'>${item.quantity}</span>
                <button class='border-0 bg-transparent' onclick='updateQty(${item.productId}, ${item.quantity + 1})'>+</button>
            </div>
            <button class='btn-remove' onclick='removeItem(${item.productId})'>
                <i class='bi bi-trash me-1'></i> Remove
            </button>
        </div>
    </div>`;
});
            itemsContainer.innerHTML = itemsHtml;

            // 2. عرض ملخص الحساب
            document.getElementById('summaryBox').innerHTML = `
                <h5 class='fw-bold mb-4'>Order Summary</h5>
                <div class='total-row d-flex justify-content-between text-dark'>
                    <span>Total</span>
                    <span>${data.total.toLocaleString()} EGP</span>
                </div>
                <button class='btn-checkout' onclick='checkout()'>Complete Order</button>
            `;
        }

        function removeItem(id) {
            window.chrome.webview.postMessage({ action: 'REMOVE', itemId: id });
        }
        function updateQty(id, newQty) {
    if (newQty < 1) return; 
    window.chrome.webview.postMessage({ 
        action: 'UPDATE_QUANTITY', 
        itemId: id, 
        quantity: newQty 
    });
}
        function checkout() {
            window.chrome.webview.postMessage({ action: 'CHECKOUT' });
        }
function goBack() {
    window.chrome.webview.postMessage({ action: 'CLOSE_FORM' });
}
    </script>
</body>
</html>";

            webView.CoreWebView2.NavigateToString(htmlContent);
        }
        private async Task LoadCartData()
        {
            // جلب بيانات السلة الحقيقية من السيرفس
            var cart = await _cartService.GetCustomerCart(UserSession.CustomerId);

            var data = new
            {
                type = "RENDER_CART",
                customerName = UserSession.CustomerName,
                items = cart.CartProducts.Select(p => new {
                    productId = p.ProductId,
                    productName = p.ProductName,
                    imagePath = p.ImagePath,
                    price = p.Price,
                    quantity = p.Quantity,
                    itemTotal = p.ItemTotal
                }).ToList(),
                total = cart.CartProducts.Sum(p => p.ItemTotal) * 1.15m
            };

            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            webView.CoreWebView2.PostWebMessageAsJson(json);
        }


        private async void OnWebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            using (JsonDocument doc = JsonDocument.Parse(e.WebMessageAsJson))
            {
                string action = doc.RootElement.GetProperty("action").GetString();

                if (action == "CLOSE_FORM")
                {
                    this.Close();
                }

                if (action == "REMOVE")
                {
                    int itemId = doc.RootElement.GetProperty("itemId").GetInt32();

                    await _cartService.RemoveItem(UserSession.CustomerId, itemId);

                    await LoadCartData();
                }
                else if (action == "UPDATE_QUANTITY") 
                {
                    int itemId = doc.RootElement.GetProperty("itemId").GetInt32();
                    int newQty = doc.RootElement.GetProperty("quantity").GetInt32();

                    await _cartService.UpdateItemQuantity(UserSession.CustomerId, itemId, newQty);

                    await LoadCartData();
                }

                else if (action == "CHECKOUT")
                {
                    MessageBox.Show("Processing your order...");
                }
            }
        }
    }
}
