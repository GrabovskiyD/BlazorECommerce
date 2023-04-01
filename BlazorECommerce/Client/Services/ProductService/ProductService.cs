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

            OnProductChanged?.Invoke();
        }

        public async Task SearchProducts(string searchText)
        {
            var result = await _httpClient.GetFromJsonAsync<ServiceResponse<List<Product>>>($"api/product/search/{searchText}");
            if (result is not null && result.Data is not null)
            {
                Products = result.Data;
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
    }
}
