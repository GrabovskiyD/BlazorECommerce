namespace BlazorECommerce.Client.Services.ProductService
{
    public interface IProductService
    {
        event Action? OnProductChanged;
        List<Product> Products { get; set; }

        Task GetProducts(string? categoryUrl = null);

        Task<ServiceResponse<Product>> GetProduct(int productId);
    }
}
