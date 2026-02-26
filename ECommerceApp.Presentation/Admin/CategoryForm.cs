using ECommerceApp.Application.DTOs.CategoryDtos;
using ECommerceApp.Application.Interfaces.Services;
using ECommerceApp.Application.Services;
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
        private readonly IOrderService _orderService;
        private readonly ICustomerUserService _customerUserService;

        public CategoryForm(ICategoryService categoryService,IProductService productService,IOrderService orderService,ICustomerUserService customerUserService)
        {
            InitializeComponent();
            _categoryService = categoryService;
            _productService = productService;
            _orderService = orderService;
            _customerUserService = customerUserService;
            this.Text = "Category Management";
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
<html lang=""en"">
<head>
<meta charset=""UTF-8"">
<meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
<link href=""https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css"" rel=""stylesheet"">
<link rel=""stylesheet"" href=""https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.0/font/bootstrap-icons.css"">

<style>
:root{
    --primary:#0d6efd;
    --bg:#f4f7f9;
    --card-bg:#ffffff;
    --text-muted:#6c757d;
}

body{
    background:var(--bg);
    font-family:'Segoe UI',sans-serif;
    margin:0;
    padding:30px;
}

.header{
    display:flex;
    justify-content:space-between;
    align-items:center;
    margin-bottom:30px;
}

.btn-add{
    background:var(--primary);
    color:white;
    border:none;
    padding:10px 20px;
    border-radius:8px;
    font-weight:600;
    transition:.2s;
}
.btn-add:hover{
    opacity:.9;
}

.category-grid{
    display:grid;
    grid-template-columns:repeat(auto-fill,minmax(280px,1fr));
    gap:25px;
}

.category-card{
    background:var(--card-bg);
    border-radius:15px;
    overflow:hidden;
    box-shadow:0 4px 15px rgba(0,0,0,.05);
    transition:.3s ease;
    display:flex;
    flex-direction:column;
}

.category-card:hover{
    transform:translateY(-5px);
    box-shadow:0 8px 25px rgba(0,0,0,.1);
}

.category-img{
    width:100%;
    height:180px;
    object-fit:cover;
}

.category-body{
    padding:20px;
    flex:1;
    display:flex;
    flex-direction:column;
    justify-content:space-between;
}

.category-title{
    font-size:18px;
    font-weight:600;
    margin-bottom:10px;
}

.category-desc{
    font-size:14px;
    color:var(--text-muted);
    margin-bottom:15px;
}

.category-actions{
    display:flex;
    justify-content:space-between;
    align-items:center;
}

.category-actions button{
    border:none;
    background:none;
    font-size:18px;
    cursor:pointer;
    transition:.2s;
}

.btn-edit:hover{ color:var(--primary); }
.btn-delete:hover{ color:#dc3545; }

.stat-box{
    background:white;
    padding:15px 20px;
    border-radius:10px;
    margin-bottom:25px;
    box-shadow:0 2px 8px rgba(0,0,0,.05);
}
.btn-back{
    background:white;
    border:1px solid #dee2e6;
    width:45px;
    height:45px;
    border-radius:10px;
    display:flex;
    align-items:center;
    justify-content:center;
    font-size:18px;
    cursor:pointer;
    transition:.2s;
}

.btn-back:hover{
    background:#f1f5f9;
    transform:translateX(-3px);
}

.category-id-badge {
    position: absolute;
    top: 10px;
    right: 10px;
    background: rgba(0, 0, 0, 0.6);
    color: white;
    padding: 2px 8px;
    border-radius: 5px;
    font-size: 12px;
    z-index: 1;
}
.category-card { position: relative; }

</style>
</head>

<body>

<div class=""header"">

    <div class=""d-flex align-items-center gap-3"">
        <button class=""btn-back"" onclick=""goBack()"">
            <i class=""bi bi-arrow-left""></i>
        </button>

        <div>
            <h2 style=""margin:0;"">Category Management</h2>
            <small class=""text-muted"">Manage your product categories easily</small>
        </div>
    </div>

    <button class=""btn-add"" onclick=""addCategory()"">
        <i class=""bi bi-plus-lg""></i> Add Category
    </button>

</div>

<div class=""stat-box"">
    <strong>Total Categories: </strong>
    <span id=""statCount"">0</span>
</div>

<div class=""category-grid"" id=""categoryContainer"">
    <!-- Categories Render Here -->
</div>

<script>
window.chrome.webview.addEventListener('message', event => {
    const data = event.data;

    if(data.action === ""LOAD_CATEGORIES""){
        const container = document.getElementById(""categoryContainer"");
        const statCount = document.getElementById(""statCount"");

        statCount.innerText = data.categories.length;

        let cards = """";

        if(data.categories.length === 0){
            container.innerHTML = ""<p>No categories found</p>"";
            return;
        }

        data.categories.forEach(cat => {
            cards += `
            <div class=""category-card"">
                <div class=""category-id-badge"">ID: ${cat.id}</div>
                <img src=""${cat.imagePath || 'https://via.placeholder.com/400x200?text=No+Image'}"" 
                     class=""category-img""
                     onerror=""this.src='https://via.placeholder.com/400x200?text=No+Image'"">

                <div class=""category-body"">
                    <div>
                        <div class=""category-title"">${cat.categoryName}</div>
                        <div class=""category-desc"">${cat.description || '-'}</div>
                    </div>

                    <div class=""category-actions"">
                        <button class=""btn-edit"" 
                            onclick='editCategory(${cat.id},""${cat.categoryName}"",""${cat.description || ''}"",""${cat.imagePath || ''}"")'>
                            <i class=""bi bi-pencil-square""></i>
                        </button>

                        <button class=""btn-delete"" 
                            onclick=""deleteCategory(${cat.id})"">
                            <i class=""bi bi-trash""></i>
                        </button>
                    </div>
                </div>
            </div>
            `;
        });

        container.innerHTML = cards;
    }
});
function goBack(){
    window.chrome.webview.postMessage({ action:""GO_BACK"" });
}
function addCategory(){
    const name = prompt(""Category Name:"");
    const desc = prompt(""Description:"");
    const image = prompt(""Image URL:"");

    if(name){
        window.chrome.webview.postMessage({
            action:""ADD_CATEGORY"",
            name:name,
            description:desc,
            image:image
        });
    }
}

function editCategory(id,name,desc,image){
    const newName = prompt(""Edit Name:"",name);
    const newDesc = prompt(""Edit Description:"",desc);
    const newImage = prompt(""Edit Image URL:"",image);

    if(newName){
        window.chrome.webview.postMessage({
            action:""UPDATE_CATEGORY"",
            id:id,
            name:newName,
            description:newDesc,
            image:newImage
        });
    }
}

function deleteCategory(id){
    if(confirm(""Delete this category?"")){
        window.chrome.webview.postMessage({
            action:""DELETE_CATEGORY"",
            id:id
        });
    }
}

window.onload = () => {
    window.chrome.webview.postMessage({action:""LOAD""});
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
                    categories = categories 
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
                        var adminForm = new DashboardForm(_categoryService,_productService,_orderService, _customerUserService);
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