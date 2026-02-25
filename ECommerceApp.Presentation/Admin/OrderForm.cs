using ECommerceApp.Application.DTOs.Order;
using ECommerceApp.Application.Interfaces.Services;
using ECommerceApp.Infrastructure.Enums;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

using System.Text.Json;


namespace ECommerceApp.Presentation.Admin
{
    public partial class OrderForm : Form
    {
        private WebView2 webView;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IOrderService _OrderService;
        public OrderForm(IOrderService orderService , IProductService productService, ICategoryService categoryService)
        {
            InitializeComponent();
            this.Text = "E-Comm Suite - Order Management";
            this.WindowState = FormWindowState.Maximized;
            _OrderService = orderService;
            _categoryService = categoryService;
            _productService = productService;
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
        :root {
            --sidebar-bg: #1e3a58;
            --main-bg: #f4f7f9;
            --text-light: #a5b4c1;
        }

        body {
            background-color: var(--main-bg);
            font-family: 'Segoe UI', sans-serif;
            margin: 0;
            display: flex;
            height: 100vh;
            overflow: hidden;
        }

        :root {
            --sidebar-dark: #1e293b;
            --bg-light: #f8fafc;
            --accent: #0f172a;
            --text-muted: #64748b;
        }

        /* Sidebar */
        .sidebar {
            width: 260px;
            background: var(--sidebar-bg);
            color: white;
            display: flex;
            flex-direction: column;
        }

        .sidebar-header {
            padding: 30px 20px;
            border-bottom: 1px solid rgba(255, 255, 255, 0.1);
        }

        .nav-link {
            color: var(--text-light);
            padding: 15px 25px;
            text-decoration: none;
            display: flex;
            align-items: center;
            gap: 15px;
            transition: 0.3s;
        }

        .nav-link:hover,
        .nav-link.active {
            background: rgba(255, 255, 255, 0.1);
            color: white;
        }

        .user-profile {
            margin-top: auto;
            padding: 20px;
            background: rgba(0, 0, 0, 0.2);
            display: flex;
            align-items: center;
            gap: 12px;
        }

        /* Content Area */
        .container {
            flex: 1;
            padding: 40px;
            overflow-y: auto;
        }

        .header {
            margin-bottom: 30px;
        }

        .header h1 {
            margin: 0;
            color: var(--text-dark);
            font-size: 24px;
        }

        .header p {
            margin: 5px 0 0;
            color: var(--text-muted);
        }

        /* Search Bar */
        .btn-back {
            background: white;
            border: 1px solid #dee2e6;
            color: var(--secondary);
            padding: 8px 15px;
            border-radius: 8px;
            cursor: pointer;
            transition: 0.2s;
        }

        .btn-back:hover {
            background: #f8f9fa;
            color: #000;
        }

        .search-box input {
            border: none;
            outline: none;
            width: 100%;
            font-size: 14px;
        }

        /* Orders Table */
        .table-card {
            background: white;
            border-radius: 12px;
            border: 1px solid #e2e8f0;
            box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.05);
        }

        table {
            width: 100%;
            border-collapse: collapse;
        }

        th {
            text-align: left;
            padding: 18px;
            background: #f8fafc;
            color: var(--text-muted);
            font-size: 12px;
            text-transform: uppercase;
            border-bottom: 1px solid #e2e8f0;
        }

        td {
            padding: 18px;
            border-bottom: 1px solid #f1f5f9;
            font-size: 14px;
            color: var(--text-dark);
        }

        /* Customer Info */
        .cust-name {
            font-weight: 600;
            display: block;
        }

        .cust-email {
            font-size: 12px;
            color: var(--text-muted);
        }

        /* Status Badges */
        .status {
            padding: 5px 12px;
            border-radius: 6px;
            font-size: 12px;
            font-weight: 500;
            display: inline-flex;
            align-items: center;
            gap: 6px;
        }

        .status::before {
            content: '';
            width: 6px;
            height: 6px;
            border-radius: 50%;
        }

        .status-processing {
            background: #fef3c7;
            color: #d97706;
        }

        .status-processing::before {
            background: #d97706;
        }

        .status-shipped {
            background: #e0e7ff;
            color: #4338ca;
        }

        .status-shipped::before {
            background: #4338ca;
        }

        .status-delivered {
            background: #dcfce7;
            color: #15803d;
        }

        .status-delivered::before {
            background: #15803d;
        }

        .action-dots {
            color: #94a3b8;
            cursor: pointer;
            font-weight: bold;
            font-size: 18px;
        }
.status-pending { background: #f1f5f9; color: #475569; } /* رمادي */
.status-completed { background: #dcfce7; color: #15803d; } /* أخضر */
.status-cancelled { background: #fee2e2; color: #b91c1c; } /* أحمر */

/* لضمان ظهور النقطة الملونة بجانب الحالات الجديدة */
.status-pending::before { background: #475569; }
.status-completed::before { background: #15803d; }
.status-cancelled::before { background: #b91c1c; }
    </style>
</head>

<body>



    <main class='container'>
        <div class='header'>
            <div class='header-title-section'>
                <button class='btn-back' onclick='goBack()' title='Return to Dashboard'>
                    <i class='bi bi-arrow-left'></i> Back
                </button>
                <div>
                    <h1 style='margin:0; font-size: 1.8rem;'>Order Management</h1>
                    <p style='color:#64748b; margin:0;'>Manage and track customer purchases in real-time.</p>
                </div>
            </div>
        </div>

        <div class='table-card'>
            <table>
                <thead>
                    <tr>
                        <th>Order ID</th>
                        <th>Customer</th>
                        <th>Date</th>
                        <th>Total Amount</th>
                        <th>Status</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody id=""ordersTable"">
                    <tr>
                        <td colspan='5' class='text-center'>Loading orders...</td>
                    </tr>
                </tbody>
            </table>
        </div>
    </main>

    <script>
window.chrome.webview.addEventListener('message', event => {
    const data = event.data;
    if (data.action === 'LOAD_ORDERS') {
        const tableBody = document.getElementById('ordersTable');
        tableBody.innerHTML = ''; 

        data.orders.forEach(order => {
const stateMap = {
    '1': 'Pending',
    '2': 'Processing',
    '3': 'Shipped',
    '4': 'Completed',
    '5': 'Cancelled'
};
            const row = `
    <tr>
        <td>#${order.id}</td>
        <td><span class='cust-name'>${order.customerName || 'N/A'}</span></td>
        <td>${new Date(order.orderDate).toLocaleDateString()}</td>
        <td>$${order.totalAmount}</td>
        <td><span class='status status-${stateMap[order.state]?.toLowerCase()}'>${stateMap[order.state]}</span></td>
        <td>
            <select class='form-select form-select-sm' onchange='updateStatus(${order.id}, this.value)'>
                <option value=''>Change Status</option>
                <option value='1'>Pending</option>
                <option value='2'>Processing</option>
                <option value='3'>Shipped</option>
                <option value='4'>Completed</option>
                <option value='5'>Cancelled</option>
            </select>
        </td>
    </tr>`;
            tableBody.innerHTML += row;
        });
    }
});

function updateStatus(id, newStatus) {
    if (newStatus) {
        window.chrome.webview.postMessage({ 
            action: ""UPDATE_ORDER"", 
            id: id, 
            state: newStatus 
        });
    }
}

function goBack() {
    window.chrome.webview.postMessage({ action: ""GO_BACK"" });
}

window.onload = () => {
    window.chrome.webview.postMessage({ action: ""LOAD"" });
};
    </script>
</body>

</html>";

            webView.CoreWebView2.NavigateToString(htmlContent);
        }
        private async Task LoadOrders()
        {
            try
            {
                var orders = await _OrderService.GetAllOrders();
                var response = new { action = "LOAD_ORDERS", orders = orders };

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                string json = JsonSerializer.Serialize(response, options);

                if (webView?.CoreWebView2 != null)
                {
                    webView.CoreWebView2.PostWebMessageAsJson(json);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading orders: {ex.Message}");
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
                        await LoadOrders();
                        break;

                    case "GO_BACK":
                        var adminForm = new DashboardForm(_categoryService, _productService, _OrderService);
                        adminForm.Show();
                        this.Close();
                        break;

                    case "UPDATE_ORDER":
                        var updateDto = new UpdateOrderDto
                        {
                            Id = doc.RootElement.GetProperty("id").GetInt32(),
                            State = doc.RootElement.GetProperty("state").GetString()
                        };
                        await _OrderService.UpdateOrderStatus(updateDto);
                        await LoadOrders();
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
