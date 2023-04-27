namespace BlazorECommerce.Server.Services.ProductTypeService
{
    public class ProductTypeService : IProductTypeService
    {
        private readonly DataContext _dataContext;

        public ProductTypeService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<ServiceResponse<List<ProductType>>> GetProductTypesAsync()
        {
            var productTypes = await _dataContext.ProductTypes.ToListAsync();
            return new ServiceResponse<List<ProductType>>
            {
                Data = productTypes,
                Success = true
            };
        }
    }
}
