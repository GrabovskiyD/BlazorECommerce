using BlazorECommerce.Shared.Model;
using System.Security.Claims;

namespace BlazorECommerce.Server.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly DataContext _dataContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartService(DataContext dataContext, IHttpContextAccessor httpContextAccessor)
        {
            _dataContext = dataContext;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

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
                    ProductTypeId = productVariant.ProductTypeId,
                    Quantity = cartItem.Quantity
                };

                result.Data.Add(cartProduct);
            }

            return result;
        }

        public async Task<ServiceResponse<List<CartProductResponseDTO>>> StoreCartItemsAsync(List<CartItem> cartItems)
        {
            cartItems.ForEach(cartItem => cartItem.UserId = GetUserId());
            _dataContext.CartItems.AddRange(cartItems);
            await _dataContext.SaveChangesAsync();

            return await GetDbCartProductsAsync();
        }

        public async Task<ServiceResponse<int>> GetCartItemsCountAsync()
        {
            var count = (await _dataContext.CartItems.Where(ci => ci.UserId == GetUserId()).ToListAsync()).Sum(ci => ci.Quantity);
            return new ServiceResponse<int>
            {
                Data = count,
                Success = true
            };
        }

        public async Task<ServiceResponse<List<CartProductResponseDTO>>> GetDbCartProductsAsync()
        {
            return await GetCartProductsAsync(await _dataContext.CartItems
                .Where(cartItem => cartItem.UserId == GetUserId()).ToListAsync());
        }

        public async Task<ServiceResponse<bool>> AddItemToCartAsync(CartItem cartItem)
        {
            cartItem.UserId = GetUserId();

            var sameItem = await _dataContext.CartItems
                .FirstOrDefaultAsync(ci => ci.ProductId == cartItem.ProductId &&
                ci.ProductTypeId == cartItem.ProductTypeId && ci.UserId == cartItem.UserId);

            if(sameItem is null)
            {
                _dataContext.CartItems.Add(cartItem);
            }
            else
            {
                sameItem.Quantity += cartItem.Quantity;
            }

            await _dataContext.SaveChangesAsync();
            return new ServiceResponse<bool> 
            {
                Data = true,
                Success = true
            };
        }

        public async Task<ServiceResponse<bool>> UpdateItemsQuantityAsync(CartItem cartItem)
        {
            var dbCartItem = await _dataContext.CartItems
                .FirstOrDefaultAsync(ci => ci.ProductId == cartItem.ProductId &&
                ci.ProductTypeId == cartItem.ProductTypeId && ci.UserId == GetUserId());
            if(dbCartItem is null)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Success = false,
                    Message = "Cart item does not exist"
                };
            }

            dbCartItem.Quantity = cartItem.Quantity;
            await _dataContext.SaveChangesAsync();

            return new ServiceResponse<bool> 
            {
                Data = true, 
                Success = true 
            };
        }

        public async Task<ServiceResponse<bool>> RemoveItemFromCartAsync(int productId, int productTypeId)
        {
            var dbCartItem = await _dataContext.CartItems
                .FirstOrDefaultAsync(ci => ci.ProductId == productId &&
                ci.ProductTypeId == productTypeId && ci.UserId == GetUserId());
            if (dbCartItem is null)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Success = false,
                    Message = "Cart item does not exist"
                };
            }

            _dataContext.CartItems.Remove(dbCartItem);
            await _dataContext.SaveChangesAsync();
            return new ServiceResponse<bool> 
            { 
                Data = true, 
                Success = true 
            };
        }
    }
}
