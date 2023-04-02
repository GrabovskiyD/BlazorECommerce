namespace BlazorECommerce.Server.Services.CartService
{
    public interface ICartService
    {
        Task<ServiceResponse<List<CartProductResponseDTO>>> GetCartProductsAsync(List<CartItem> cartItems);
    }
}
