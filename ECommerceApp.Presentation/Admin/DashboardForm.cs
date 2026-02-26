using ECommerceApp.Application.Interfaces.Services;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.Text.Json;

namespace ECommerceApp.Presentation.Admin
{
    public partial class DashboardForm : Form
    {
        private WebView2 webView;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly ICustomerUserService _customerUserService;

        public DashboardForm(ICategoryService categoryService, IProductService productService, IOrderService orderService, ICustomerUserService customerUserService)
        {
            InitializeComponent();
            this.Text = "E-Comm Suite - Administrator Dashboard";
            this.WindowState = FormWindowState.Maximized;

            _categoryService = categoryService;
            _productService = productService;
            _orderService = orderService;
            _customerUserService = customerUserService;

            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            webView = new WebView2 { Dock = DockStyle.Fill };
            this.Controls.Add(webView);

            await webView.EnsureCoreWebView2Async(null);

            // ربط الأحداث
            webView.CoreWebView2.WebMessageReceived += OnWebMessageReceived;

            // تحديث البيانات تلقائياً بعد انتهاء تحميل الصفحة
            webView.NavigationCompleted += async (s, e) => {
                await LoadDashboardDataFromService();
            };

            string htmlContent = GetHtmlTemplate();
            webView.NavigateToString(htmlContent);
        }

        private async Task LoadDashboardDataFromService()
        {
            try
            {
                // جلب البيانات من السيرفيس
                var stats = await _categoryService.GetDashboard();

                // تحويل الكائن إلى JSON
                string jsonStats = JsonSerializer.Serialize(stats);

                // إرسال البيانات إلى JavaScript لتحديث الواجهة
                await webView.CoreWebView2.ExecuteScriptAsync($"updateDashboard({jsonStats})");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading dashboard data: {ex.Message}");
            }
        }

        private void OnWebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            using (JsonDocument doc = JsonDocument.Parse(e.WebMessageAsJson))
            {
                string action = doc.RootElement.GetProperty("action").GetString();
                if (action == "NAVIGATE")
                {
                    string page = doc.RootElement.GetProperty("page").GetString();
                    HandleNavigation(page);
                }
                else if (action == "LOGOUT")
                {
                    var result = MessageBox.Show("Are you sure you want to log out?", "Logout",
                                               MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        System.Windows.Forms.Application.Exit();
                    }
                }
            }
        }

        private void HandleNavigation(string page)
        {
            switch (page)
            {
                case "categories":
                    new CategoryForm(_categoryService, _productService, _orderService, _customerUserService).Show();
                    break;
                case "products":
                    new ProductForm(_productService, _categoryService, _orderService, _customerUserService).Show();
                    break;
                case "orders":
                    new OrderForm(_orderService, _productService, _categoryService, _customerUserService).Show();
                    break;
                case "users":
                    new CustomerForm(_customerUserService).Show();
                    break;
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
        :root { --sidebar-bg: #1e3a58; --main-bg: #f4f7f9; --text-light: #a5b4c1; }
        body { background-color: var(--main-bg); font-family: 'Segoe UI', sans-serif; margin: 0; display: flex; height: 100vh; overflow: hidden; }
        .sidebar { width: 260px; background: var(--sidebar-bg); color: white; display: flex; flex-direction: column; }
        .sidebar-header { padding: 30px 20px; border-bottom: 1px solid rgba(255,255,255,0.1); }
        .nav-link { color: var(--text-light); padding: 15px 25px; text-decoration: none; display: flex; align-items: center; gap: 15px; transition: 0.3s; }
        .nav-link:hover, .nav-link.active { background: rgba(255,255,255,0.1); color: white; }
        .content { flex: 1; overflow-y: auto; padding: 40px; }
        .stat-card { background: white; border-radius: 12px; padding: 25px; border: none; box-shadow: 0 4px 12px rgba(0,0,0,0.03); height: 100%; }
        .stat-icon { width: 45px; height: 45px; border-radius: 10px; background: #f0f4f8; display: flex; align-items: center; justify-content: center; font-size: 1.2rem; color: var(--sidebar-bg); }
        .user-profile { margin-top: auto; padding: 20px; background: rgba(0,0,0,0.2); }
    </style>
</head>
<body>
    <div class='sidebar'>
        <div class='sidebar-header d-flex align-items-center gap-2'>
            <div class='bg-light text-dark p-2 rounded'><i class='bi bi-shop'></i></div>
            <div>
                <div class='fw-bold' style='font-size: 0.9rem;'>ADMIN PANEL</div>
                <div style='font-size: 0.7rem; color: var(--text-light);'>E-Commerce System</div>
            </div>
        </div>
        <nav class='mt-3'>
            <a href='#' class='nav-link active' onclick='navigate(""dashboard"")'><i class='bi bi-grid'></i> Dashboard</a>
            <a href='#' class='nav-link' onclick='navigate(""categories"")'><i class='bi bi-tags'></i> Categories</a>
            <a href='#' class='nav-link' onclick='navigate(""products"")'><i class='bi bi-box-seam'></i> Products</a>
            <a href='#' class='nav-link' onclick='navigate(""orders"")'><i class='bi bi-cart'></i> Orders</a>
            <a href='#' class='nav-link' onclick='navigate(""users"")'><i class='bi bi-people'></i> Users</a>
        </nav>
        <div class='user-profile text-center'>
            <button class='btn btn-link text-danger p-0 w-100' onclick='logout()' style='text-decoration: none;'>
                <i class='bi bi-box-arrow-right'></i> Log Out
            </button>
        </div>
    </div>

    <div class='content'>
        <div class='d-flex justify-content-between align-items-center mb-4'>
            <h4 class='fw-bold m-0'>Administrator Dashboard</h4>
            <small class='text-muted'>Last Updated: <span id='lastUpdated'>-</span></small>
        </div>
        
        <div class='row g-4'>
            <div class='col-md-3'>
                <div class='stat-card d-flex align-items-center justify-content-between'>
                    <div><div class='text-muted small'>TOTAL PRODUCTS</div><h3 class='fw-bold m-0' id='totalProducts'>0</h3></div>
                    <div class='stat-icon'><i class='bi bi-box'></i></div>
                </div>
            </div>
            <div class='col-md-3'>
                <div class='stat-card d-flex align-items-center justify-content-between'>
                    <div><div class='text-muted small'>TOTAL CATEGORIES</div><h3 class='fw-bold m-0' id='totalCategories'>0</h3></div>
                    <div class='stat-icon'><i class='bi bi-diagram-3'></i></div>
                </div>
            </div>
            <div class='col-md-3'>
                <div class='stat-card d-flex align-items-center justify-content-between'>
                    <div><div class='text-muted small'>TOTAL ORDERS</div><h3 class='fw-bold m-0' id='totalOrders'>0</h3></div>
                    <div class='stat-icon'><i class='bi bi-bag'></i></div>
                </div>
            </div>
            <div class='col-md-3'>
                <div class='stat-card d-flex align-items-center justify-content-between'>
                    <div><div class='text-muted small'>TOTAL SALES</div><h3 class='fw-bold m-0' id='totalSales'>$0</h3></div>
                    <div class='stat-icon'><i class='bi bi-cash-stack'></i></div>
                </div>
            </div>

            <div class='col-md-6'>
                <div class='stat-card'>
                    <div class='text-muted small mb-2'>TOP PERFORMING CATEGORY</div>
                    <div class='d-flex align-items-center gap-3'>
                        <div class='stat-icon bg-primary text-white'><i class='bi bi-star'></i></div>
                        <h4 class='fw-bold m-0' id='topPerforming'>-</h4>
                    </div>
                </div>
            </div>
            <div class='col-md-6'>
                <div class='stat-card'>
                    <div class='text-muted small mb-2'>ACTIVE PRODUCTS IN CATEGORIES</div>
                    <div class='d-flex align-items-center gap-3'>
                        <div class='stat-icon bg-success text-white'><i class='bi bi-check-circle'></i></div>
                        <h4 class='fw-bold m-0' id='activeProducts'>0</h4>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script>
        function logout() { window.chrome.webview.postMessage({ action: 'LOGOUT' }); }
        function navigate(page) { window.chrome.webview.postMessage({ action: 'NAVIGATE', page: page }); }

        function updateDashboard(data) {
            document.getElementById('totalProducts').innerText = data.TotalProducts.toLocaleString();
            document.getElementById('totalCategories').innerText = data.TotalCategories.toLocaleString();
            document.getElementById('totalOrders').innerText = data.TotalOrders.toLocaleString();
            document.getElementById('totalSales').innerText = '$' + data.TotalSales.toLocaleString();
            document.getElementById('topPerforming').innerText = data.TopPerforming;
            document.getElementById('activeProducts').innerText = data.ActiveProducts.toLocaleString();
            
            if(data.LastUpdated) {
                const date = new Date(data.LastUpdated);
                document.getElementById('lastUpdated').innerText = date.toLocaleString();
            }
        }
    </script>
</body>
</html>";
        }
    }
}
