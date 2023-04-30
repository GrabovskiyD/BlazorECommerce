using BlazorECommerce.Shared.Model.DTOs;

namespace BlazorECommerce.Client.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        public event Action? OnProductChanged;

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public List<Product> Products { get; set; } = new List<Product>();
        public string Message { get; set; } = "Loading Products...";
        public int CurrentPage { get; set; } = 1;
        public int PageCount { get; set; } = 0;
        public string LastSearchText { get; set; } = string.Empty;
        public List<Product> AdminProducts { get; set; } = new List<Product>();

        public async Task<ServiceResponse<Product>> GetProduct(int productId)
        {
            var result = await _httpClient.GetFromJsonAsync<ServiceResponse<Product>>($"api/product/{productId}");
            return result;
        }

        public async Task GetProducts(string? categoryUrl = null)
        {
            var result = categoryUrl is null ?            
                await _httpClient.GetFromJsonAsync<ServiceResponse<List<Product>>>("api/product/featured") :
                await _httpClient.GetFromJsonAsync<ServiceResponse<List<Product>>>($"api/product/category/{categoryUrl}");

            if (result is not null && result.Data is not null)
            {
                Products = result.Data;
            }

            CurrentPage = 1;
            PageCount = 1;
            if(Products.Count == 0)
            {
                Message = "No products found.";
            }

            OnProductChanged?.Invoke();
        }

        public async Task SearchProducts(string searchText, int page )
        {
            LastSearchText = searchText;
            var result = await _httpClient.GetFromJsonAsync<ServiceResponse<ProductSearchResultDTO>>($"api/product/search/{searchText}/{page}");
            if (result is not null && result.Data is not null)
            {
                Products = result.Data.Products;
                CurrentPage = result.Data.CurrentPage;
                PageCount = result.Data.Pages;
            }
            if (Products.Count == 0)
            {
                Message = "No products found";
            }
            OnProductChanged?.Invoke();
        }

        public async Task<List<string>> GetProductsSearchSuggestions(string searchText)
        {
            var result = await _httpClient.GetFromJsonAsync<ServiceResponse<List<string>>>($"api/product/searchsuggestions/{searchText}");
            return result.Data;
        }

        public async Task GetAdminProducts()
        {
            var result = await _httpClient.GetFromJsonAsync<ServiceResponse<List<Product>>>("api/product/admin");
            AdminProducts = result.Data;
            CurrentPage = 1;
            PageCount = 0;
            if(AdminProducts.Count == 0)
            {
                Message = "No products found.";
            }
        }

        public async Task<Product> CreateProduct(Product product)
        {
            var result = await _httpClient.PostAsJsonAsync("api/product", product);
            var newProduct = (await result.Content.ReadFromJsonAsync<ServiceResponse<Product>>()).Data;
            return newProduct;
        }

        public async Task<Product> UpdateProduct(Product product)
        {
            var result = await _httpClient.PutAsJsonAsync("api/product", product);
            return (await result.Content.ReadFromJsonAsync<ServiceResponse<Product>>()).Data;
        }

        public async Task DeleteProduct(Product product)
        {
            var result = await _httpClient.DeleteAsync($"/api/product/{product.Id}");
        }
    }
}
