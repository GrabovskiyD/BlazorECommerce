namespace BlazorECommerce.Server.Services.ProductTypeService
{
    public interface IProductTypeService
    {
        Task<ServiceResponse<List<ProductType>>> GetProductTypesAsync();
        Task<ServiceResponse<List<ProductType>>> AddProductTypesAsync(ProductType productType);
        Task<ServiceResponse<List<ProductType>>> UpdateProductTypesAsync(ProductType productType);

    }
}
