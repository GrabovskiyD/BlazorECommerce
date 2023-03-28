namespace BlazorECommerce.Server.Services.ProductService
{
    public interface IProductService
    {
        public Task<ServiceResponse<List<Product>>> GetProductsAsync();
    }
}
