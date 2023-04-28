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

        public async Task AddProductTypesAsync(ProductType productType)
        {
            var response = await _httpClient.PostAsJsonAsync("api/ProductType", productType);
            ProductTypes = (await response.Content.ReadFromJsonAsync<ServiceResponse<List<ProductType>>>()).Data;
            OnChange?.Invoke();
        }

        public ProductType CreateNewProductType()
        {
            var newProductType = new ProductType
            {
                IsNew = true,
                Editing = true
            };
            ProductTypes.Add(newProductType);
            OnChange?.Invoke();
            return newProductType;
        }

        public async Task GetProductTypesAsync()
        {
            var result = await _httpClient
                .GetFromJsonAsync<ServiceResponse<List<ProductType>>>("api/ProductType");
            ProductTypes = result.Data;
        }

        public async Task UpdateProductTypesAsync(ProductType productType)
        {
            var response = await _httpClient.PutAsJsonAsync("api/ProductType", productType);
            ProductTypes = (await response.Content.ReadFromJsonAsync<ServiceResponse<List<ProductType>>>()).Data;
            OnChange?.Invoke();
        }
    }
}
