namespace BlazorECommerce.Server.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly DataContext _dataContext;

        public CartService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<ServiceResponse<List<CartProductResponseDTO>>> GetCartProductsAsync(List<CartItem> cartItems)
        {
            var result = new ServiceResponse<List<CartProductResponseDTO>>()
            {
                Data = new List<CartProductResponseDTO>()
            };

            foreach (var cartItem in cartItems)
            {
                var product = await _dataContext.Products
                    .Where(p => p.Id == cartItem.ProductId)
                    .FirstOrDefaultAsync();

                if (product is null)
                {
                    continue;
                }

                var productVariant = await _dataContext.ProductVariants
                    .Where(v => v.ProductId == cartItem.ProductId
                    && v.ProductTypeId == cartItem.ProductTypeId)
                    .Include(v => v.ProductType)
                    .FirstOrDefaultAsync();

                if (productVariant is null)
                {
                    continue;
                }

                var cartProduct = new CartProductResponseDTO
                {
                    ProductId = product.Id,
                    Title = product.Title,
                    ImageUrl = product.ImageUrl,
                    Price = productVariant.Price,
                    ProductType = productVariant.ProductType.Name,
                    ProductTypeId = productVariant.ProductTypeId
                };

                result.Data.Add(cartProduct);
            }

            return result;
        }
    }
}
