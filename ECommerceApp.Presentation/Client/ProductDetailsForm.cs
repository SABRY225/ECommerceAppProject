using ECommerceApp.Application.DTOs.ProductDtos;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.Text.Json;

namespace ECommerceApp.Presentation.Client
{
    public partial class ProductDetailsForm : Form
    {
        private WebView2 webView;
        private readonly GetProductDetailsDto _product;

        public ProductDetailsForm(GetProductDetailsDto product)
        {
            _product = product;
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.Text = "Product Details - " + _product.ProductName;
            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            webView = new WebView2 { Dock = DockStyle.Fill };
            this.Controls.Add(webView);

            await webView.EnsureCoreWebView2Async(null);
            webView.CoreWebView2.WebMessageReceived += OnWebMessageReceived;

            // استدعاء الدالة التي تقوم بتفريغ البيانات في القالب
            string finalHtml = GetHtmlTemplate();
            webView.CoreWebView2.NavigateToString(finalHtml);
        }

        // الدالة المسؤولة عن تفريغ البيانات داخل قالب HTML
        private string GetHtmlTemplate()
        {
            return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css' rel='stylesheet'>
    <link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.0/font/bootstrap-icons.css'>
    <style>
        /* الستايل الجديد لزر الرجوع */
        .back-nav {{
            margin-bottom: 20px;
        }}
        .btn-back {{
            background: white;
            border: 1px solid #e2e8f0;
            padding: 8px 18px;
            border-radius: 10px;
            color: #475569;
            font-weight: 600;
            text-decoration: none;
            display: inline-flex;
            align-items: center;
            gap: 8px;
            transition: all 0.2s;
            cursor: pointer;
        }}
        .btn-back:hover {{
            background: #f8fafc;
            color: #1e293b;
            box-shadow: 0 4px 6px -1px rgba(0,0,0,0.05);
        }}

        body {{ background: #f4f7f6; font-family: 'Segoe UI', sans-serif; padding: 40px; display: flex; flex-direction: column; align-items: center; }}
        .details-container {{ max-width: 1100px; width: 100%; }}
        /* باقي الستايل الخاص بك كما هو ... */
        .details-card {{
            background: white; border-radius: 25px; overflow: hidden; 
            box-shadow: 0 20px 40px rgba(0,0,0,0.08); display: flex; flex-wrap: wrap;
        }}
        .img-section {{ background: #fbfbfb; flex: 1; min-width: 400px; display: flex; align-items: center; justify-content: center; padding: 20px; }}
        .img-section img {{ max-width: 100%; height: auto; border-radius: 15px; }}
        .info-section {{ flex: 1.2; padding: 50px; min-width: 400px; }}
        .category-badge {{ background: #4361ee15; color: #4361ee; padding: 6px 16px; border-radius: 50px; font-size: 0.85rem; font-weight: 600; text-transform: uppercase; }}
        .product-title {{ font-size: 2.8rem; font-weight: 800; color: #1e293b; margin: 20px 0 10px 0; }}
        .price {{ font-size: 2.2rem; color: #059669; font-weight: 700; margin-bottom: 25px; }}
        .description-title {{ font-size: 1.1rem; font-weight: 700; color: #475569; margin-bottom: 10px; }}
        .description {{ color: #64748b; line-height: 1.8; font-size: 1.05rem; margin-bottom: 35px; }}
        .btn-add {{ background: #1e293b; color: white; border: none; padding: 15px 30px; border-radius: 12px; font-weight: 600; width: 100%; }}
    </style>
</head>
<body>
    <div class='details-container'>
        <div class='back-nav'>
            <div class='btn-back' onclick='goBack()'>
                <i class='bi bi-arrow-left'></i> Back to Products
            </div>
        </div>

        <div class='details-card'>
            <div class='img-section'>
                <img src='{_product.ImagePath}' onerror=""this.src='https://via.placeholder.com/500x500?text=No+Image'"">
            </div>
            <div class='info-section'>
                <span class='category-badge'>{(_product.CategoryName ?? "General")}</span>
                <h1 class='product-title'>{_product.ProductName}</h1>
                <div class='price'>{_product.Price:N2} EGP</div>
                
                <div class='description-title'>Overview</div>
                <p class='description'>{(_product.Description ?? "No detailed description provided.")}</p>
                
                <button class='btn-add' onclick='addToCart()'>
                    <i class='bi bi-cart-plus me-2'></i> Add to Shopping Bag
                </button>
            </div>
        </div>
    </div>

    <script>
        function goBack() {{
            window.chrome.webview.postMessage({{ action: 'BACK' }});
        }}
        
        function addToCart() {{
            window.chrome.webview.postMessage({{ action: 'ADD', quantity: 1 }});
        }}
    </script>
</body>
</html>";
        }

        private void OnWebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(e.WebMessageAsJson))
                {
                    string action = doc.RootElement.GetProperty("action").GetString();

                    if (action == "BACK")
                    {
                        this.Close(); 
                    }
                    else if (action == "ADD")
                    {
                        MessageBox.Show($"Success: {_product.ProductName} has been added to your cart.", "Cart Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}