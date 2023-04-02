

using BlazorECommerce.Shared.Model;

namespace BlazorECommerce.Client.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly ILocalStorageService _localStorage;

        public CartService(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }
        public event Action OnChange;

        public async Task AddToCartAsync(CartItem cartItem)
        {
            var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            if(cart is null)
            {
                cart = new List<CartItem>();
            }
            cart.Add(cartItem);

            await _localStorage.SetItemAsync("cart", cart);
            OnChange?.Invoke();
        }

        public async Task<List<CartItem>> GetCartItemsAsync()
        {
            var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            if (cart is null)
            {
                cart = new List<CartItem>();
            }

            return cart;
        }
    }
}
