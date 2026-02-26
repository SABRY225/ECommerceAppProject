using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;
using System.Text.Json;
using ECommerceApp.Application.Interfaces.Services;

namespace ECommerceApp.Presentation.Admin
{
    public partial class CustomerForm : Form
    {
        private WebView2 webView;
        private readonly ICustomerUserService _customerService;
        public CustomerForm(ICustomerUserService customerService)
        {
            InitializeComponent();
            _customerService = customerService;
            this.Text = "E-Comm Suite - Customer Management";
            this.WindowState = FormWindowState.Maximized;
            InitializeWebView();
        }
        private async void InitializeWebView()
        {
            webView = new WebView2 { Dock = DockStyle.Fill };
            this.Controls.Add(webView);

            await webView.EnsureCoreWebView2Async(null);
            webView.CoreWebView2.DOMContentLoaded += async (s, e) => {
                await LoadCustomersData();
            };

            webView.CoreWebView2.WebMessageReceived += OnWebMessageReceived;

            string htmlContent = @"
            <!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css' rel='stylesheet'>
    <link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.0/font/bootstrap-icons.css'>
    <style>
        :root { --sidebar-bg: #1e3a58; --main-bg: #f4f7f9; --text-light: #a5b4c1; }
        body { background-color: var(--main-bg); font-family: 'Segoe UI', sans-serif; margin: 0; display: flex; height: 100vh; overflow: hidden; }
        :root { --sidebar-dark: #1e293b; --bg-light: #f8fafc; --accent: #0f172a; --text-muted: #64748b; }
        
        /* Sidebar */
        .sidebar { width: 260px; background: var(--sidebar-bg); color: white; display: flex; flex-direction: column; }
        .sidebar-header { padding: 30px 20px; border-bottom: 1px solid rgba(255,255,255,0.1); }
        .nav-link { color: var(--text-light); padding: 15px 25px; text-decoration: none; display: flex; align-items: center; gap: 15px; transition: 0.3s; }
        .nav-link:hover, .nav-link.active { background: rgba(255,255,255,0.1); color: white; }
        .user-profile { margin-top: auto; padding: 20px; background: rgba(0,0,0,0.2); display: flex; align-items: center; gap: 12px; }

           /* Main Content */
        .main { flex: 1; padding: 40px; overflow-y: auto; }
        .header h1 { margin: 0; color: var(--text-main); font-size: 24px; }
        
        /* Search Bar */
        .search-container { background: white; border: 1px solid #e2e8f0; border-radius: 10px; padding: 12px 20px; margin: 25px 0; display: flex; align-items: center; gap: 10px; }
        .search-container input { border: none; outline: none; width: 100%; font-size: 14px; color: var(--text-main); }

        /* Table Styles */
        .table-card { background: white; border-radius: 12px; border: 1px solid #e2e8f0; box-shadow: 0 4px 6px -1px rgba(0,0,0,0.05); overflow: hidden; }
        table { width: 100%; border-collapse: collapse; }
        th { text-align: left; padding: 18px; background: #f8fafc; color: var(--text-muted); font-size: 12px; text-transform: uppercase; border-bottom: 1px solid #e2e8f0; }
        td { padding: 18px; border-bottom: 1px solid #f1f5f9; vertical-align: middle; }

        /* Customer Avatar & Info */
        .customer-cell { display: flex; align-items: center; gap: 15px; }
        .avatar { width: 40px; height: 40px; border-radius: 50%; background: #e2e8f0; display: flex; align-items: center; justify-content: center; font-weight: bold; color: #475569; font-size: 14px; }
        .name { display: block; font-weight: 600; color: var(--text-main); }
        .email { font-size: 12px; color: var(--text-muted); }
        
        .join-date { color: var(--text-muted); font-size: 14px; }
        .total-orders { font-weight: bold; text-align: center; color: var(--text-main); }

.btn-back {
    background: white;
    border: 1px solid #e2e8f0;
    padding: 8px 15px;
    border-radius: 8px;
    color: var(--sidebar-bg);
    cursor: pointer;
    font-weight: 600;
    display: inline-flex;
    align-items: center;
    gap: 8px;
    margin-bottom: 20px;
    transition: 0.2s;
}
.btn-back:hover {
    background: #f8fafc;
    border-color: #cbd5e1;
}
    </style>
</head>
<body>
    <main class='main'>
  <div class='header'>
    <button class='btn-back' onclick='goBack()'>
        <i class='bi bi-arrow-left'></i> Back to Dashboard
    </button>
    <h1>Customer Management</h1>
</div>

        <div class='table-card'>
            <table id='customerTable'>
                <thead>
                    <tr>
                        <th>Customer</th>
                        <th>Join Date</th>
                        <th style='text-align: center;'>Total Orders</th>
                    </tr>
                </thead>
                <tbody id=""customerTableBody""> </tbody>
            </table>
        </div>
    </main>

    <script>
        window.chrome.webview.addEventListener('message', event => {
        const message = event.data;
        if (message.type === 'RENDER_CUSTOMERS') {
            renderTable(message.payload);
        }
    });

    function renderTable(customers) {
        const tbody = document.getElementById('customerTableBody');
        tbody.innerHTML = ''; // تفريغ الجدول

        customers.forEach(customer => {
            const row = `
                <tr>
                    <td>
                        <div class='customer-cell'>
                            <div class='avatar'>${customer.initials || '??'}</div>
                            <div>
                                <span class='name'>${customer.name}</span>
                                <span class='email'>${customer.email}</span>
                            </div>
                        </div>
                    </td>
                    <td class='join-date'>${customer.joinDate}</td>
                    <td class='total-orders'>${customer.totalOrders}</td>
                </tr>
            `;
            tbody.innerHTML += row;
        });
    }
function goBack() {
    window.chrome.webview.postMessage({ action: 'CLOSE' });
}
    function navigate(page) {
        window.chrome.webview.postMessage({ action: 'NAVIGATE', page: page });
    }

    </script>
</body>
</html>";

            webView.CoreWebView2.NavigateToString(htmlContent);


        }
        private async Task LoadCustomersData()
        {
            var customers = await _customerService.Customers();

            var data = new
            {
                type = "RENDER_CUSTOMERS",
                payload = customers.Select(c => new {
                    name = c.Name,
                    email = c.Email,
                    initials = string.Concat(c.Name.Where(char.IsUpper)),
                    joinDate = c.JoinDate.ToString("MMM dd, yyyy"),
                    totalOrders = c.TotalOrders
                })
            };

            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            webView.CoreWebView2.PostWebMessageAsJson(json);
        }
        private void OnWebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            using (JsonDocument doc = JsonDocument.Parse(e.WebMessageAsJson))
            {
                if (doc.RootElement.TryGetProperty("action", out JsonElement actionElement))
                {
                    string action = actionElement.GetString();

                    if (action == "CLOSE")
                    {
                        this.Invoke(new Action(() => this.Close()));
                    }
                }
            }
        }

    }
}
