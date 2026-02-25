using ECommerceApp.Application.DTOs.ProductDtos;
using ECommerceApp.Application.Interfaces.Services;
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

        // تم إضافة ICategoryService هنا لمنع الخطأ عند العودة للخلف
        public ProductForm(IProductService productService, ICategoryService categoryService)
        {
            InitializeComponent();
            _productService = productService;
            _categoryService = categoryService;

            this.Text = "E-Comm Suite - Product Management";
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
        :root { --sidebar-bg: #1e3a58; --main-bg: #f4f7f9; --primary: #0d6efd; }
        body { background-color: var(--main-bg); font-family: 'Segoe UI', sans-serif; margin: 0; display: flex; height: 100vh; overflow: hidden; }
        .main { flex: 1; padding: 40px; overflow-y: auto; }
        .header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 30px; }
        .btn-back { background: white; border: 1px solid #dee2e6; padding: 8px 15px; border-radius: 8px; cursor: pointer; }
        .btn-add { background: var(--primary); color: white; border: none; padding: 10px 25px; border-radius: 6px; font-weight: bold; }
        .card { background: white; padding: 20px; border-radius: 12px; box-shadow: 0 1px 3px rgba(0,0,0,0.1); margin-bottom: 20px; }
        table { width: 100%; background: white; border-radius: 12px; overflow: hidden; border-collapse: collapse; }
        th { background: #f1f5f9; padding: 15px; color: #475569; font-size: 13px; text-transform: uppercase; }
        td { padding: 15px; border-bottom: 1px solid #f1f5f9; vertical-align: middle; }
        .product-img { width: 50px; height: 50px; object-fit: cover; border-radius: 8px; background: #eee; }
        .actions button { background: none; border: none; cursor: pointer; font-size: 18px; margin-right: 10px; }
        .btn-delete { color: #ef4444; }
    </style>
</head>
<body>
    <div class='main'>
        <div class='header'>
            <div>
                <button class='btn-back' onclick='goBack()'><i class='bi bi-arrow-left'></i> Back</button>
                <h1 class='mt-2' style='font-size: 1.8rem;'>Product Catalog</h1>
            </div>
            <button class='btn-add' onclick='addProduct()'><i class='bi bi-plus-lg'></i> Add Product</button>
        </div>

        <div class='card' style='width: 200px;'>
            <small>Total Products</small>
            <h2 id='statCount'>0</h2>
        </div>

        <table>
            <thead>
                <tr>
                    <th>ID</th>
                    <th>Image</th>
                    <th>Product Name</th>
                    <th>Category</th>
                    <th>Price</th>
                    <th>Stock</th>
                    <th>Actions</th>
                </tr>
            </thead>
            <tbody id='productTable'>
                <tr><td colspan='7' class='text-center'>Loading...</td></tr>
            </tbody>
        </table>
    </div>

    <script>
        window.chrome.webview.addEventListener('message', event => {
            const data = event.data;
            if (data.action === 'LOAD_PRODUCTS') {
                renderTable(data.products);
            }
        });

        function renderTable(products) {
            const table = document.getElementById('productTable');
            document.getElementById('statCount').innerText = products.length;

            if (products.length === 0) {
                table.innerHTML = '<tr><td colspan=7 class=text-center>No products found.</td></tr>';
                return;
            }

            table.innerHTML = products.map(p => `
                <tr>
                    <td style='color:#94a3b8'>#${p.id}</td>
                    <td><img src='${p.imagePath}' class='product-img' onerror=""this.src='https://via.placeholder.com/50'""></td>
                    <td><b>${p.productName}</b><br><small class='text-muted'>${p.description || ''}</small></td>
                    <td><span class='badge bg-light text-dark'>${p.categoryName || 'N/A'}</span></td>
                    <td>$${p.price.toFixed(2)}</td>
                    <td>${p.stockQuantity}</td>
                    <td class='actions'>
                        <button onclick='editProduct(${JSON.stringify(p)})'>✎</button>
                        <button class='btn-delete' onclick='deleteProduct(${p.id})'>🗑</button>
                    </td>
                </tr>
            `).join('');
        }

        function addProduct() {
            const name = prompt('Product Name:');
            const desc = prompt('Description:');
            const price = parseFloat(prompt('Price:'));
            const stock = parseInt(prompt('Stock:'));
            const catId = parseInt(prompt('Category ID:'));
            const img = prompt('Image URL:');

            if (name && !isNaN(price)) {
                window.chrome.webview.postMessage({
                    action: 'ADD_PRODUCT',
                    productName: name,
                    description: desc,
                    price: price,
                    stockQuantity: stock,
                    categoryId: catId,
                    imagePath: img
                });
            }
        }

        function editProduct(p) {
            const name = prompt('Edit Name:', p.productName);
            const price = parseFloat(prompt('Edit Price:', p.price));
            const stock = parseInt(prompt('Edit Stock:', p.stockQuantity));
            const img = prompt('Edit Image URL:', p.imagePath);

            if (name) {
                window.chrome.webview.postMessage({
                    action: 'UPDATE_PRODUCT',
                    id: p.id,
                    productName: name,
                    description: p.description,
                    price: price,
                    stockQuantity: stock,
                    categoryId: p.categoryId,
                    imagePath: img
                });
            }
        }

        function deleteProduct(id) {
            if (confirm('Delete this product?')) {
                window.chrome.webview.postMessage({ action: 'DELETE_PRODUCT', id: id });
            }
        }

        function goBack() { window.chrome.webview.postMessage({ action: 'GO_BACK' }); }
        
        window.onload = () => { window.chrome.webview.postMessage({ action: 'LOAD' }); };
    </script>
</body>
</html>";

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
                        var adminForm = new DashboardForm(_categoryService, _productService);
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