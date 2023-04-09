using BlazorECommerce.Shared.Model.DTOs;

namespace BlazorECommerce.Client.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public CartService(ILocalStorageService localStorage,
            HttpClient httpClient,
            AuthenticationStateProvider authenticationStateProvider)
        {
            _localStorage = localStorage;
            _httpClient = httpClient;
            _authenticationStateProvider = authenticationStateProvider;
        }
        public event Action OnChange;

        public async Task AddToCartAsync(CartItem cartItem)
        {
            if (await IsUserAuthenticated())
            {
                await _httpClient.PostAsJsonAsync("api/cart/add", cartItem);
            }
            else
            {
                var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
                if (cart is null)
                {
                    cart = new List<CartItem>();
                }

                var sameItem = cart.Find(x => x.ProductId == cartItem.ProductId &&
                    x.ProductTypeId == cartItem.ProductTypeId);
                if (sameItem is null)
                {
                    cart.Add(cartItem);
                }
                else
                {
                    sameItem.Quantity += cartItem.Quantity;
                }
                await _localStorage.SetItemAsync("cart", cart);
            }
            await GetCartItemsCountAsync();
        }

        private async Task<bool> IsUserAuthenticated()
        {
            return (await _authenticationStateProvider.GetAuthenticationStateAsync()).User.Identity.IsAuthenticated;
        }

        public async Task<List<CartProductResponseDTO>> GetCartProductsAsync()
        {
            if (await IsUserAuthenticated())
            {
                var response = await _httpClient.GetFromJsonAsync<ServiceResponse<List<CartProductResponseDTO>>>("api/cart");
                return response.Data;
            }
            else
            {
                var cartItems = await _localStorage.GetItemAsync<List<CartItem>>("cart");
                if (cartItems is null)
                {
                    return new List<CartProductResponseDTO>();
                }
                var response = await _httpClient.PostAsJsonAsync("api/cart/products", cartItems);
                var cartProducts =
                    await response.Content.ReadFromJsonAsync<ServiceResponse<List<CartProductResponseDTO>>>();
                return cartProducts.Data;
            }


        }

        public async Task RemoveProductFromCartAsync(int productId, int productTypeId)
        {
            if (await IsUserAuthenticated())
            {
                await _httpClient.DeleteAsync($"api/cart/{productId}/{productTypeId}");
            }
            else
            {
                var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");

                if (cart is null)
                {
                    return;
                }

                var cartItem = cart.Find(x => x.ProductId == productId
                    && x.ProductTypeId == productTypeId);

                if (cartItem is not null)
                {
                    cart.Remove(cartItem);
                    await _localStorage.SetItemAsync("cart", cart);
                }
            }
        }

        public async Task StoreCartItemsAsync(bool emptyLocalCart = false)
        {
            var localCart = await _localStorage.GetItemAsync<List<CartItem>>("cart");

            if (localCart is null)
            {
                return;
            }
            await _httpClient.PostAsJsonAsync("api/cart", localCart);

            if (emptyLocalCart)
            {
                await _localStorage.RemoveItemAsync("cart");
            }
        }

        public async Task UpdateQuantityAsync(CartProductResponseDTO product)
        {
            if (await IsUserAuthenticated())
            {
                var request = new CartItem
                {
                    ProductId = product.ProductId,
                    Quantity = product.Quantity,
                    ProductTypeId = product.ProductTypeId
                };
                await _httpClient.PutAsJsonAsync("api/cart/update-quantity", request);
            }
            else
            {
                var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
                if (cart is null)
                {
                    return;
                }

                var cartItem = cart.Find(x => x.ProductId == product.ProductId
                    && x.ProductTypeId == product.ProductTypeId);
                if (cartItem is not null)
                {
                    cartItem.Quantity = product.Quantity;
                    await _localStorage.SetItemAsync("cart", cart);
                }
            }
            await GetCartItemsCountAsync();
        }

        public async Task GetCartItemsCountAsync()
        {
            if (await IsUserAuthenticated())
            {
                var result = await _httpClient.GetFromJsonAsync<ServiceResponse<int>>("api/cart/count");
                var count = result.Data;

                await _localStorage.SetItemAsync<int>("cartItemsCount", count);
            }
            else
            {
                var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
                await _localStorage.SetItemAsync<int>("cartItemsCount", cart != null ? cart.Count : 0);
            }

            OnChange.Invoke();
        }
    }
}
