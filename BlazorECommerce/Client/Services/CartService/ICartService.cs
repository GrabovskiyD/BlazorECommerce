using BlazorECommerce.Shared.Model.DTOs;

namespace BlazorECommerce.Client.Services.CartService
{
    public interface ICartService
    {
        event Action OnChange;
        Task AddToCartAsync(CartItem cartItem);
        Task<List<CartItem>> GetCartItemsAsync();
        Task<List<CartProductResponseDTO>> GetCartProductsAsync();
        Task RemoveProductFromCartAsync(int productId, int productTypeId);
        Task UpdateQuantityAsync(CartProductResponseDTO product);
        Task StoreCartItemsAsync(bool emptyLocalCart);
        Task GetCartItemsCountAsync();
    }
}
