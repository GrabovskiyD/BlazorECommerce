using static System.Net.WebRequestMethods;

namespace BlazorECommerce.Client.Services.ProductTypeService
{
    public class ProductTypeService : IProductTypeService
    {
        private readonly HttpClient _httpClient;

        public ProductTypeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public List<ProductType> ProductTypes { get; set; } = new List<ProductType>();

        public event Action OnChange;

        public async Task GetProductTypesAsync()
        {
            var result = await _httpClient
                .GetFromJsonAsync<ServiceResponse<List<ProductType>>>("api/ProductType");
            ProductTypes = result.Data;
        }
    }
}
