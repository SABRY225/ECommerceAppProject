using ECommerceApp.Application.DTOs.CategoryDtos;
using ECommerceApp.Application.Interfaces.Services;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.Text.Json;

namespace ECommerceApp.Presentation.Admin
{
    public partial class CategoryForm : Form
    {
        private WebView2 webView;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;

        public CategoryForm(ICategoryService categoryService)
        {
            InitializeComponent();
            _categoryService = categoryService;
            this.Text = "Category Management";
            this.WindowState = FormWindowState.Maximized;
            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            webView = new WebView2 { Dock = DockStyle.Fill };
            this.Controls.Add(webView);

            // تهيئة المحرك
            await webView.EnsureCoreWebView2Async(null);

            // ربط حدث استقبال الرسائل قبل تحميل المحتوى
            webView.CoreWebView2.WebMessageReceived += OnWebMessageReceived;

            string htmlContent = @"
            <!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css' rel='stylesheet'>
    <link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.0/font/bootstrap-icons.css'>
    <style>
        :root { --sidebar-bg: #1e3a58; --main-bg: #f4f7f9; --text-light: #a5b4c1; --primary: #0d6efd; }
        body { background-color: var(--main-bg); font-family: 'Segoe UI', sans-serif; margin: 0; display: flex; height: 100vh; overflow: hidden; }
        .sidebar { width: 260px; background: var(--sidebar-bg); color: white; display: flex; flex-direction: column; }
        .btn-back { 
                        background: white; border: 1px solid #dee2e6; color: var(--secondary); 
                        padding: 8px 15px; border-radius: 8px; cursor: pointer; transition: 0.2s; 
                    }
                    .btn-back:hover { background: #f8f9fa; color: #000; }
        .sidebar-header { padding: 30px 20px; border-bottom: 1px solid rgba(255,255,255,0.1); }
        .nav-link { color: var(--text-light); padding: 15px 25px; text-decoration: none; display: flex; align-items: center; gap: 15px; transition: 0.3s; }
        .nav-link:hover, .nav-link.active { background: rgba(255,255,255,0.1); color: white; }
        .main { flex: 1; padding: 40px; overflow-y: auto; }
        .header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 30px; }
        .btn-add { background: var(--primary); color: white; border: none; padding: 10px 25px; border-radius: 6px; cursor: pointer; font-weight: bold; }
        .card { background: white; padding: 20px; border-radius: 12px; box-shadow: 0 1px 3px rgba(0,0,0,0.1); }
        .stats { display: grid; grid-template-columns: repeat(4, 1fr); gap: 20px; margin-bottom: 30px; }
        table { width: 100%; border-collapse: collapse; background: white; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 6px -1px rgba(0,0,0,0.1); }
        th { text-align: left; background: #f1f5f9; padding: 15px; color: #475569; font-size: 13px; }
        td { padding: 15px; border-bottom: 1px solid #f1f5f9; color: #334155; }
        .badge { background: #e2e8f0; padding: 5px 12px; border-radius: 20px; font-weight: bold; font-size: 12px; }
        .actions button { background: none; border: none; cursor: pointer; color: #64748b; margin-right: 10px; font-size: 18px; }
        .actions button:hover { color: var(--primary); }
        .actions .btn-delete:hover { color: #ef4444; }
    </style>
</head>
<body>
    <div class='main'>
              <div class='header'>
                        <div class='header-title-section'>
                            <button class='btn-back' onclick='goBack()' title='Return to Dashboard'>
                                <i class='bi bi-arrow-left'></i> Back
                            </button>
                            <div>
                                <h1 style='margin:0; font-size: 1.8rem;'>Category Management</h1>
                                <p style='color:#64748b; margin:0;'>View and manage your product categories</p>
                            </div>
                        </div>
                        <button class='btn-add' onclick='addCategory()'>
                            <i class='bi bi-plus-lg'></i> Add New Category
                        </button>
                    </div>

        <div class='stats'>
            <div class='card'><small>Total Categories</small><h2 id='statCount'>0</h2></div>
        </div>

        <table>
            <thead>
                <tr><th>ID</th><th>NAME</th><th>DESCRIPTION</th><th>Image</th><th>ACTIONS</th></tr>
            </thead>
            <tbody id=""categoryTable"">
                <tr><td colspan='4' class='text-center'>Loading categories...</td></tr>
            </tbody>
        </table>
    </div>

    <script>
        // استقبال البيانات من C#
        window.chrome.webview.addEventListener('message', event => {
            const data = event.data;
            if (data.action === ""LOAD_CATEGORIES"") {
                const table = document.getElementById(""categoryTable"");
                const statCount = document.getElementById(""statCount"");
                let rows = """";
                
                statCount.innerText = data.categories.length;

                if(data.categories.length === 0) {
                    rows = ""<tr><td colspan='4' class='text-center'>No categories found</td></tr>"";
                } else {
                    data.categories.forEach(cat => {
                        rows += `
                            <tr>
                                <td style='color:#94a3b8'>#${cat.id}</td>
                                <td><b>${cat.categoryName || 'No Name'}</b></td>
                                <td>${cat.description || '-'}</td>                                
                                <td>${cat.imagePath || '-'}</td>
                                <td class='actions'>
                                    <button onclick='editCategory(${cat.id}, ""${cat.categoryName}"", ""${cat.description || ''}"",""${cat.imagePath}"")'>✎</button>
                                    <button class='btn-delete' onclick='deleteCategory(${cat.id})'>🗑</button>
                                </td>
                            </tr>`;
                    });
                }
                table.innerHTML = rows;
            }
        });

        function addCategory() {
            const name = prompt(""Enter Category Name:"");
            const desc = prompt(""Enter Description:"");            
            const image = prompt(""Enter Image Url:"");

            if (name) {
                window.chrome.webview.postMessage({ action: ""ADD_CATEGORY"", name: name, description: desc,image:image });
            }
        }

        function editCategory(id, oldName, oldDesc,oldImage) {
            const name = prompt(""Edit Name:"", oldName);
            const desc = prompt(""Edit Description:"", oldDesc);            
            const image = prompt(""Edit Image url:"", oldImage);

            if (name) {
                window.chrome.webview.postMessage({ action: ""UPDATE_CATEGORY"", id: id, name: name, description: desc,image:image });
            }
        }

        function deleteCategory(id) {
            if (confirm(""Are you sure you want to delete this category?"")) {
                window.chrome.webview.postMessage({ action: ""DELETE_CATEGORY"", id: id });
            }
        }
        function goBack() {
                        window.chrome.webview.postMessage({ action: ""GO_BACK"" });
         }
 
        // إرسال طلب تحميل البيانات بمجرد جاهزية الصفحة
        window.onload = () => {
            window.chrome.webview.postMessage({ action: ""LOAD"" });
        };
    </script>
</body>
</html>";

            webView.CoreWebView2.NavigateToString(htmlContent);
        }

        private async Task LoadCategories()
        {
            try
            {
                var categories = await _categoryService.GetAll();

                var data = new
                {
                    action = "LOAD_CATEGORIES",
                    categories = categories // ستحول التلقائياً لـ camelCase بسبب الإعدادات أدناه
                };

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };

                string json = JsonSerializer.Serialize(data, options);

                if (webView?.CoreWebView2 != null)
                {
                    await webView.InvokeAsyncSafe(() => {
                        webView.CoreWebView2.PostWebMessageAsJson(json);
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
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
                        await LoadCategories();
                        break;

                    case "GO_BACK":
                        var adminForm = new DashboardForm(_categoryService, _productService);
                        adminForm.Show();
                        this.Hide();
                        break;

                    case "ADD_CATEGORY":
                        var addDto = new AddCategoryDto
                        {
                            CategoryName = doc.RootElement.GetProperty("name").GetString(),
                            Description = doc.RootElement.GetProperty("description").GetString(),
                            ImagePath = doc.RootElement.GetProperty("image").GetString()
                        };
                        await _categoryService.Add(addDto);
                        await LoadCategories();
                        break;

                    case "UPDATE_CATEGORY":
                        var updateDto = new UpdateCategoryDto
                        {
                            Id = doc.RootElement.GetProperty("id").GetInt32(),
                            CategoryName = doc.RootElement.GetProperty("name").GetString(),
                            Description = doc.RootElement.GetProperty("description").GetString(),
                            ImagePath = doc.RootElement.GetProperty("image").GetString()
                        };
                        await _categoryService.Update(updateDto);
                        await LoadCategories();
                        break;

                    case "DELETE_CATEGORY":
                        int id = doc.RootElement.GetProperty("id").GetInt32();
                        await _categoryService.Delete(id);
                        await LoadCategories();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"JS-C# Communication Error: {ex.Message}");
            }
        }

    }


    public static class WebViewExtensions
    {
        public static Task InvokeAsyncSafe(this Control control, Action action)
        {
            if (control.InvokeRequired)
            {
                var tcs = new TaskCompletionSource<object>();
                control.BeginInvoke(new MethodInvoker(() => {
                    try { action(); tcs.SetResult(null); }
                    catch (Exception ex) { tcs.SetException(ex); }
                }));
                return tcs.Task;
            }
            action();
            return Task.CompletedTask;
        }
    }
}