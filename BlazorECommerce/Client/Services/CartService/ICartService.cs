using BlazorECommerce.Shared.Model.DTOs;

namespace BlazorECommerce.Client.Services.CartService
{
    public interface ICartService
    {
        event Action OnChange;
        Task AddToCartAsync(CartItem cartItem);
        Task<List<CartItem>> GetCartItemsAsync();
        Task<List<CartProductResponseDTO>> GetCartProductsAsync();
    }
}
