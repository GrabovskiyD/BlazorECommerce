namespace BlazorECommerce.Server.Services.CartService
{
    public interface ICartService
    {
        Task<ServiceResponse<List<CartProductResponseDTO>>> GetCartProductsAsync(List<CartItem> cartItems);
        Task<ServiceResponse<List<CartProductResponseDTO>>> StoreCartItemsAsync(List<CartItem> cartItems);
        Task<ServiceResponse<int>> GetCartItemsCountAsync();
        Task<ServiceResponse<List<CartProductResponseDTO>>> GetDbCartProductsAsync();
        Task<ServiceResponse<bool>> AddItemToCartAsync(CartItem cartItem);
        Task<ServiceResponse<bool>> UpdateItemsQuantityAsync(CartItem cartItem);
    }
}
