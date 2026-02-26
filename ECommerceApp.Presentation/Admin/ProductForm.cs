using ECommerceApp.Application.DTOs.ProductDtos;
using ECommerceApp.Application.Interfaces.Services;
using ECommerceApp.Application.Services;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.Text.Json;

namespace ECommerceApp.Presentation.Admin
{
    public partial class ProductForm : Form
    {
        private WebView2 webView;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IOrderService _orderService;
        private readonly ICustomerUserService _customerUserService;

        public ProductForm(IProductService productService, ICategoryService categoryService, IOrderService orderService ,ICustomerUserService customerUserService)
        {
            InitializeComponent();
            _productService = productService;
            _categoryService = categoryService;
            _customerUserService = customerUserService;
            this.Text = "E-Comm Suite - Product Management";
            this.WindowState = FormWindowState.Maximized;
            InitializeWebView();
            _orderService = orderService;
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
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css' rel='stylesheet'>
    <link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.0/font/bootstrap-icons.css'>
    <style>
        :root { 
            --primary: #4361ee; 
            --success: #4cc9f0; 
            --danger: #f72585; 
            --dark: #212529;
            --light-bg: #f8f9fa;
        }
        body { background-color: #f3f4f7; font-family: 'Inter', 'Segoe UI', sans-serif; color: #444; }
        
        /* Container Layout */
        .main-container { padding: 30px; max-width: 1400px; margin: auto; }
        
        /* Header Section */
        .page-header { 
            display: flex; justify-content: space-between; align-items: center; 
            margin-bottom: 30px; background: white; padding: 20px; border-radius: 15px;
            box-shadow: 0 4px 6px -1px rgba(0,0,0,0.1);
        }
        
        /* Stats Card */
        .stat-badge {
            background: var(--primary); color: white; padding: 5px 15px;
            border-radius: 20px; font-weight: 600; font-size: 0.9rem;
        }

        /* Product Table Styling */
        .table-container {
            background: white; border-radius: 15px; overflow: hidden;
            box-shadow: 0 10px 15px -3px rgba(0,0,0,0.05);
        }
        .table { margin-bottom: 0; vertical-align: middle; }
        .table thead { background-color: #f8f9fa; border-bottom: 2px solid #edf2f7; }
        .table th { padding: 18px; font-weight: 600; color: #64748b; text-transform: uppercase; font-size: 0.75rem; letter-spacing: 0.05em; }
        .table td { padding: 16px; border-bottom: 1px solid #f1f5f9; }

        /* Product Info */
        .product-cell { display: flex; align-items: center; gap: 15px; }
        .product-img { 
            width: 55px; height: 55px; object-fit: cover; border-radius: 12px;
            border: 1px solid #eee; transition: transform 0.2s;
        }
        .product-img:hover { transform: scale(1.1); }
        .product-name { font-weight: 600; color: var(--dark); margin: 0; font-size: 0.95rem; }
        .product-desc { font-size: 0.8rem; color: #94a3b8; display: block; }

        /* Status & Badges */
        .price-tag { font-weight: 700; color: var(--primary); font-size: 1rem; }
        .stock-badge { 
            padding: 5px 12px; border-radius: 8px; font-size: 0.8rem; font-weight: 600;
        }
        .stock-low { background: #fff1f2; color: #e11d48; }
        .stock-ok { background: #f0fdf4; color: #16a34a; }

        /* Action Buttons */
        .btn-action {
            width: 35px; height: 35px; border-radius: 10px; border: none;
            display: inline-flex; align-items: center; justify-content: center;
            transition: all 0.2s; margin-left: 5px;
        }
        .btn-edit { background: #eef2ff; color: #4361ee; }
        .btn-edit:hover { background: #4361ee; color: white; }
        .btn-delete { background: #fff1f2; color: #e11d48; }
        .btn-delete:hover { background: #e11d48; color: white; }
        
        .btn-add-new {
            background: var(--primary); color: white; border: none; padding: 10px 20px;
            border-radius: 10px; font-weight: 600; display: flex; align-items: center; gap: 8px;
            box-shadow: 0 4px 14px 0 rgba(67, 97, 238, 0.39);
        }
    </style>
</head>
<body>
    <div class='main-container'>
        <div class='page-header'>
            <div>
                <button class='btn btn-light btn-sm mb-2' onclick='goBack()'>
                    <i class='bi bi-arrow-left'></i> Back to Dashboard
                </button>
                <h2 class='fw-bold m-0'>Inventory Management <span id='statCount' class='ms-2 badge bg-primary rounded-pill' style='font-size: 1rem;'>0</span></h2>
            </div>
            <button class='btn-add-new' onclick='addProduct()'>
                <i class='bi bi-plus-circle-fill'></i> Create New Product
            </button>
        </div>

        <div class='table-container'>
            <table class='table'>
                <thead>
                    <tr>
                        <th>Product Details</th>
                        <th>Category</th>
                        <th>Price</th>
                        <th>Availability</th>
                        <th class='text-end'>Actions</th>
                    </tr>
                </thead>
                <tbody id='productTable'>
                    <tr><td colspan='5' class='text-center py-5 text-muted'>Initialising...</td></tr>
                </tbody>
            </table>
        </div>
    </div>

    <script>
        window.chrome.webview.addEventListener('message', event => {
            const data = event.data;
            if (data.action === 'LOAD_PRODUCTS') renderTable(data.products);
        });

        function renderTable(products) {
            const table = document.getElementById('productTable');
            document.getElementById('statCount').innerText = products.length;

            if (products.length === 0) {
                table.innerHTML = '<tr><td colspan=5 class=text-center py-5>No products in your inventory.</td></tr>';
                return;
            }

            table.innerHTML = products.map(p => {
                const stockClass = p.stockQuantity < 10 ? 'stock-low' : 'stock-ok';
                return `
                <tr>
                    <td>
                        <div class='product-cell'>
                            <img src='${p.imagePath}' class='product-img' onerror=""""this.src='https://cdn-icons-png.flaticon.com/512/679/679821.png'"""">
                            <div>
                                <p class='product-name'>${p.productName}</p>
                                <span class='product-desc'>${p.description ? p.description.substring(0, 40) + '...' : 'No description'}</span>
                            </div>
                        </div>
                    </td>
                    <td><span class='badge bg-light text-dark border'>${p.categoryName || 'General'}</span></td>
                    <td><span class='price-tag'>$${p.price.toLocaleString(undefined, {minimumFractionDigits: 2})}</span></td>
                    <td><span class='stock-badge ${stockClass}'><i class='bi bi-box-seam me-1'></i> ${p.stockQuantity} in stock</span></td>
                    <td class='text-end'>
                        <button class='btn-action btn-edit' onclick='editProduct(${JSON.stringify(p)})' title='Edit'>
                            <i class='bi bi-pencil-square'></i>
                        </button>
                        <button class='btn-action btn-delete' onclick='deleteProduct(${p.id})' title='Delete'>
                            <i class='bi bi-trash3'></i>
                        </button>
                    </td>
                </tr>`;
            }).join('');
        }

        function addProduct() {
             // ملاحظة: يمكنك هنا فتح Modal بدلاً من الـ prompt في المستقبل
            const name = prompt('Product Name:');
            if (!name) return;
            const price = parseFloat(prompt('Price:'));
            const stock = parseInt(prompt('Stock:'));
            const catId = parseInt(prompt('Category ID (1, 2, 3...):'));
            const img = prompt('Image URL:');

            window.chrome.webview.postMessage({
                action: 'ADD_PRODUCT',
                productName: name,
                description: '',
                price: price,
                stockQuantity: stock,
                categoryId: catId,
                imagePath: img
            });
        }

        function editProduct(p) {
            const name = prompt('Edit Name:', p.productName);
            if (name) {
                window.chrome.webview.postMessage({
                    action: 'UPDATE_PRODUCT',
                    id: p.id,
                    productName: name,
                    description: p.description,
                    price: parseFloat(prompt('Edit Price:', p.price)),
                    stockQuantity: parseInt(prompt('Edit Stock:', p.stockQuantity)),
                    categoryId: p.categoryId,
                    imagePath: prompt('Edit Image URL:', p.imagePath)
                });
            }
        }

        function deleteProduct(id) {
            if (confirm('Are you sure you want to remove this product?')) {
                window.chrome.webview.postMessage({ action: 'DELETE_PRODUCT', id: id });
            }
        }

        function goBack() { window.chrome.webview.postMessage({ action: 'GO_BACK' }); }
        window.onload = () => { window.chrome.webview.postMessage({ action: 'LOAD' }); };
    </script>
</body>
</html>"";";

            webView.CoreWebView2.NavigateToString(htmlContent);
        }

        private async Task LoadProducts()
        {
            try
            {
                var products = await _productService.GetAll();
                var response = new { action = "LOAD_PRODUCTS", products = products };

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                string json = JsonSerializer.Serialize(response, options);

                if (webView?.CoreWebView2 != null)
                {
                    webView.CoreWebView2.PostWebMessageAsJson(json);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}");
            }
        }

        private async void OnWebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                using JsonDocument doc = JsonDocument.Parse(e.WebMessageAsJson);
                string action = doc.RootElement.GetProperty("action").GetString();

                switch (action)
                {
                    case "LOAD":
                        await LoadProducts();
                        break;

                    case "GO_BACK":
                        var adminForm = new DashboardForm(_categoryService, _productService,_orderService, _customerUserService);
                        adminForm.Show();
                        this.Close();
                        break;

                    case "ADD_PRODUCT":
                        var addDto = new AddProductDto
                        {
                            ProductName = doc.RootElement.GetProperty("productName").GetString(),
                            Description = doc.RootElement.GetProperty("description").GetString(),
                            ImagePath = doc.RootElement.GetProperty("imagePath").GetString(),
                            StockQuantity = doc.RootElement.GetProperty("stockQuantity").GetInt32(),
                            Price = decimal.Parse(doc.RootElement.GetProperty("price").ToString()),
                            CategoryId = doc.RootElement.GetProperty("categoryId").GetInt32()
                        };
                        await _productService.Add(addDto);
                        await LoadProducts();
                        break;

                    case "UPDATE_PRODUCT":
                        var updateDto = new UpdateProductDto
                        {
                            Id = doc.RootElement.GetProperty("id").GetInt32(),
                            ProductName = doc.RootElement.GetProperty("productName").GetString(),
                            Description = doc.RootElement.GetProperty("description").GetString(),
                            ImagePath = doc.RootElement.GetProperty("imagePath").GetString(),
                            StockQuantity = int.Parse(doc.RootElement.GetProperty("stockQuantity").ToString()),
                            Price = decimal.Parse(doc.RootElement.GetProperty("price").ToString()),
                            CategoryId = int.Parse(doc.RootElement.GetProperty("categoryId").ToString()),
                        };
                        await _productService.Update(updateDto);
                        await LoadProducts();
                        break;

                    case "DELETE_PRODUCT":
                        int id = doc.RootElement.GetProperty("id").GetInt32();
                        await _productService.Delete(id);
                        await LoadProducts();
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Operation Error: {ex.Message}");
            }
        }
    }
}