namespace BlazorECommerce.Server.Services.ProductTypeService
{
    public class ProductTypeService : IProductTypeService
    {
        private readonly DataContext _dataContext;

        public ProductTypeService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<ServiceResponse<List<ProductType>>> AddProductTypesAsync(ProductType productType)
        {
            productType.Editing = productType.IsNew = false;
            _dataContext.ProductTypes.Add(productType);
            await _dataContext.SaveChangesAsync();

            return await GetProductTypesAsync();
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

        public async Task<ServiceResponse<List<ProductType>>> UpdateProductTypesAsync(ProductType productType)
        {
            var dbProductType = await _dataContext.ProductTypes
                .FindAsync(productType.Id);

            if(dbProductType is null)
            {
                return new ServiceResponse<List<ProductType>>
                {
                    Success = false,
                    Message = "Product Type not found"
                };
            }
            dbProductType.Name = productType.Name;
            await _dataContext.SaveChangesAsync();

            return await GetProductTypesAsync();
        }
    }
}
