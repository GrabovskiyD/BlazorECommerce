namespace BlazorECommerce.Client.Services.ProductTypeService
{
    public class ProductTypeService : IProductTypeService
    {
        private readonly HttpClient _httpClient;

        public ProductTypeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public List<ProductType> ProductTypes { get; set; }

        public event Action OnChange;

        public async Task GetProdyctTypesAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<List<ProductType>>>("/api/producttype");
            ProductTypes = response.Data;
        }
    }
}
