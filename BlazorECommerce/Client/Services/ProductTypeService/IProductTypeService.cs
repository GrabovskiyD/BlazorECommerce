namespace BlazorECommerce.Client.Services.ProductTypeService
{
    public interface IProductTypeService
    {
        event Action OnChange;
        public List<ProductType> ProductTypes { get; set; }
        Task GetProductTypesAsync();
        Task AddProductTypesAsync(ProductType productType);
        Task UpdateProductTypesAsync(ProductType productType);
        ProductType CreateNewProductType();
    }
}
