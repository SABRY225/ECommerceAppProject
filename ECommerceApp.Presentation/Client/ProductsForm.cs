using ECommerceApp.Application.DTOs.Cart;
using ECommerceApp.Application.Interfaces.Services;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.Text.Json;

namespace ECommerceApp.Presentation.Client
{
    public partial class ProductsForm : Form
    {
        private WebView2 webView;
        private readonly IProductService _productService;
        private readonly IOrderService _OrderService;
        private readonly ICartService _cartService;
        public ProductsForm(IProductService productService, IOrderService orderService, ICartService cartService)
        {
            InitializeComponent();
            _productService = productService;
            _OrderService = orderService;
            _cartService = cartService;
            this.Text = "E-Comm Suite - Products";
            this.WindowState = FormWindowState.Maximized;

            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            webView = new WebView2 { Dock = DockStyle.Fill };
            this.Controls.Add(webView);

            await webView.EnsureCoreWebView2Async(null);
            webView.CoreWebView2.WebMessageReceived += OnWebMessageReceived;

            string htmlContent = @"<!DOCTYPE html>
<html lang='en'>
<head>
<meta charset='UTF-8'>
<meta name='viewport' content='width=device-width, initial-scale=1.0'>
<link href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css' rel='stylesheet'>
<link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.0/font/bootstrap-icons.css'>

<style>
:root { --dark-blue:#1e3a58; --light-bg:#f4f6f9; }

body {
    background-color: var(--light-bg);
    font-family:'Segoe UI',sans-serif;
    padding-bottom:100px;
}

.navbar {
    background:white;
    border-bottom:1px solid #eee;
    padding:12px 30px;
}

.search-container {
    padding:20px 40px;
}

.search-input {
    width:100%;
    padding:12px 20px;
    border-radius:10px;
    border:1px solid #ddd;
    background:white;
    outline:none;
}

.product-card {
    background:white;
    border-radius:15px;
    overflow:hidden;
    border:none;
    box-shadow:0 4px 10px rgba(0,0,0,0.03);
    transition:0.3s;
}
.product-card:hover {
    transform:translateY(-5px);
    box-shadow:0 8px 20px rgba(0,0,0,0.08);
}

.product-img-wrapper {
    background:#000;
    height:250px;
    display:flex;
    align-items:center;
    justify-content:center;
}

.product-img {
    max-width:100%;
    max-height:100%;
    object-fit:contain;
}

.product-info {
    padding:20px;
}

.product-title {
    font-size:1rem;
    font-weight:500;
    margin-bottom:8px;
}

.product-price {
    color:var(--dark-blue);
    font-weight:bold;
    font-size:1.1rem;
}

.btn-add-cart {
    width:100%;
    background:var(--dark-blue);
    color:white;
    border:none;
    padding:10px;
    border-radius:8px;
    margin-top:15px;
}

.bottom-bar {
    position:fixed;
    bottom:0;
    left:0;
    right:0;
    background:white;
    padding:15px 40px;
    border-top:1px solid #eee;
    display:flex;
    justify-content:space-between;
    align-items:center;
}
.btn-link:hover {
    transform: scale(1.1);
    color: #dc3545 !important;
}
</style>
</head>

<body>

<nav class='navbar d-flex justify-content-between align-items-center shadow-sm'>
    <div class='d-flex align-items-center gap-2'  style='cursor:pointer'>
        <div class='bg-dark text-white p-2 rounded'><i class='bi bi-shop'></i></div>
        <span class='fw-bold text-dark-blue'>E-Comm Suite</span>
    </div>

    <div class='d-flex align-items-center gap-4'>
        <nav class='navbar d-flex justify-content-between align-items-center shadow-sm'>
    <div class='d-flex align-items-center gap-4'>
        <button class='btn btn-link text-danger p-0' onclick='logout()' title='Logout'>
            <i class='bi bi-box-arrow-right fs-4'></i>
        </button>

        <button class='btn btn-outline-primary d-flex align-items-center gap-2 shadow-sm' onclick='goToOrders()'>
            <i class='bi bi-bag-check-fill'></i>
            <span>My Orders</span>
        </button>
        </div>
</nav>

        <div class='customer-profile d-flex align-items-center gap-2'>
            <div class='text-end d-none d-md-block'>
                <small class='text-muted d-block' style='font-size: 0.7rem;'>Welcome,</small>
                <span class='fw-bold' id='customerName' style='color: var(--dark-blue);'>Guest User</span>
            </div>
            <div class='avatar-circle'>
                <i class='bi bi-person-circle' style='font-size: 1.5rem; color: var(--dark-blue);'></i>
            </div>
        </div>
    </div>
</nav>

<div class='search-container'>
    <input type='text' id='searchInput' class='search-input' 
           placeholder='Find the product you want...' 
           oninput='filterProducts()'>
</div>

<div class='container-fluid px-5'>
    <div class='row g-4' id='product-list'></div>
</div>

<div class='bottom-bar shadow'>
    <div>
        <i class=""bi bi-cart3 me-2""></i>
        <strong>Items Count:</strong> <span id='totalPrice'>0</span> 
    </div>
    <button class=""btn btn-primary"" onclick=""checkout()"">Checkout</button>
</div>

<script>
let allProducts = []; 

// مستمع رسائل واحد لكل العمليات
window.chrome.webview.addEventListener('message', event => {
    const data = event.data;

    switch(data.type) {
        case 'RENDER_PRODUCTS':
            allProducts = data.products; 
            displayProducts(allProducts);
            break;
            
        case 'SET_CUSTOMER_NAME':
            document.getElementById('customerName').innerText = data.name;
            break;

        case 'UPDATE_CART_UI':
            const totalPriceElement = document.getElementById('totalPrice');
            if (totalPriceElement) {
                totalPriceElement.innerText = data.itemsCount.toLocaleString();
            }
            break;
    }
});

function displayProducts(products) {
    const container = document.getElementById('product-list');
    if (!products || products.length === 0) {
        container.innerHTML = '<div class=""col-12 text-center text-muted py-5"">No products found.</div>';
        return;
    }

    let cards = '';
    products.forEach(product => {
        cards += `
        <div class='col-12 col-sm-6 col-md-4'>
            <div class='product-card' onclick='viewDetails(${product.id})' style='cursor:pointer'>
                <div class='product-img-wrapper'>
                    <img src='${product.imagePath || ""https://via.placeholder.com/300""}' 
                         class='product-img' 
                         onerror=""this.src='https://via.placeholder.com/300'"">
                </div>
                <div class='product-info text-center'>
                    <div class='product-title'>${product.productName}</div>
                    <div class='product-price'>${product.price} EGP</div>
                    <button class='btn-add-cart' onclick='event.stopPropagation(); addToCart(${product.id})'>
                        <i class='bi bi-cart-plus'></i> Add To Cart
                    </button>
                </div>
            </div>
        </div>`;
    });
    container.innerHTML = cards;
}

function filterProducts() {
    const searchTerm = document.getElementById('searchInput').value.toLowerCase();
    const filtered = allProducts.filter(p => p.productName.toLowerCase().includes(searchTerm));
    displayProducts(filtered);
}

function addToCart(productId) {
    window.chrome.webview.postMessage({ type: 'ADD_TO_CART', id: productId });
}

function viewDetails(productId) {
    window.chrome.webview.postMessage({ type: 'VIEW_DETAILS', id: productId });
}

function goToOrders() {
    window.chrome.webview.postMessage({ type: 'GO_TO_ORDERS' });
}

function logout() {
    if(confirm('Are you sure you want to logout?')) {
        window.chrome.webview.postMessage({ type: 'LOGOUT' });
    }
}
function checkout() {
    window.chrome.webview.postMessage({ 
        type: 'OPEN_CART' 
    });
}
window.onload = () => {
    window.chrome.webview.postMessage({ type: 'LOAD_PRODUCTS' });
};
</script>

</body>
</html>";

            webView.CoreWebView2.NavigateToString(htmlContent);
        }

        private async Task LoadProducts()
        {
            var products = await _productService.GetAll();

            var data = new
            {
                type = "RENDER_PRODUCTS",
                products = products
            };

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            string json = JsonSerializer.Serialize(data, options);

            webView.CoreWebView2.PostWebMessageAsJson(json);
        }
        private async Task LoadCart()
        {
            var cart = await _cartService.GetCustomerCart(UserSession.CustomerId);
            if(cart != null)
            {
                var itemsCount = cart.CartProducts.Sum(p => p.Quantity);
                var data = new
                {
                    type = "UPDATE_CART_UI",
                    itemsCount = itemsCount,
                };

                string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                webView.CoreWebView2.PostWebMessageAsJson(json);
            }
            else
            {
                var data = new
                {
                    type = "UPDATE_CART_UI",
                    itemsCount = 0,
                };

                string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                webView.CoreWebView2.PostWebMessageAsJson(json);
            }

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

            switch (type)
            {
                case "LOAD_PRODUCTS":
                    await LoadProducts();
                    await LoadCart();
                    SetCustomerInfo();
                    break;

                case "ADD_TO_CART":
                    int id = doc.RootElement.GetProperty("id").GetInt32();
                    AddProductToCartDto dataProductdto = new AddProductToCartDto
                    {
                        ProductId = id,
                        CustomerId = UserSession.CustomerId,
                    };
                    await _cartService.AddToCart(dataProductdto);
                    await LoadCart();
                    MessageBox.Show($"تم إضافة المنتج رقم {id} إلى السلة بنجاح!");
                    break;
                case "GO_TO_ORDERS":
                     var ordersForm = new MyOrdersForm(_OrderService);
                     ordersForm.Show();
                    break;
                case "VIEW_DETAILS":
                    int productId = doc.RootElement.GetProperty("id").GetInt32();
                    var p=await _productService.GetProductDetails(productId);
                    using (var productDetailsForm = new ProductDetailsForm(p, _cartService))
                    {
                        productDetailsForm.ShowDialog();
                    }
                    await LoadCart();
                    break;
                case "OPEN_CART":
                    var cartForm = new CartForm(_cartService, _OrderService);

                    cartForm.FormClosed += async (s, args) =>
                    {
                        await LoadCart(); 
                    };

                    cartForm.Show();
                    break;
                case "LOGOUT":
                    UserSession.CustomerId = 0;
                    UserSession.CustomerName = string.Empty;
                    this.Hide(); 
                    var loginForm = new LoginForm();
                    loginForm.Show();
                    //this.Close(); 
                    break;
            }
        }
    }
}