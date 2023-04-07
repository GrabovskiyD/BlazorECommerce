

using BlazorECommerce.Shared.Model;
using BlazorECommerce.Shared.Model.DTOs;
using Microsoft.AspNetCore.Components.Authorization;

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
            if((await _authenticationStateProvider.GetAuthenticationStateAsync()).User.Identity.IsAuthenticated)
            {
                Console.WriteLine("user is authenticated");
            }
            else
            {
                Console.WriteLine("user is NOT authencticated");
            }
            var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            if(cart is null)
            {
                cart = new List<CartItem>();
            }

            var sameItem = cart.Find(x => x.ProductId == cartItem.ProductId && 
            x.ProductTypeId == cartItem.ProductTypeId);
            if(sameItem is null)
            {
                cart.Add(cartItem);
            }
            else
            {
                sameItem.Quantity += cartItem.Quantity;
            }

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

        public async Task<List<CartProductResponseDTO>> GetCartProductsAsync()
        {
            var cartItems = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            var response = await _httpClient.PostAsJsonAsync("api/cart/products", cartItems);
            var cartProducts = 
                await response.Content.ReadFromJsonAsync<ServiceResponse<List<CartProductResponseDTO>>>();
            return cartProducts.Data;

        }

        public async Task RemoveProductFromCartAsync(int productId, int productTypeId)
        {
            var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            if(cart is null)
            {
                return;
            }

            var cartItem = cart.Find(x => x.ProductId == productId
                && x.ProductTypeId == productTypeId);
            if(cartItem is not null)
            {
                cart.Remove(cartItem);
                await _localStorage.SetItemAsync("cart", cart);
                OnChange?.Invoke();
            }
        }

        public async Task StoreCartItemsAsync(bool emptyLocalCart = false)
        {
            var localCart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            if(localCart is null)
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
    }
}
