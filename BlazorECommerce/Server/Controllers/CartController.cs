using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlazorECommerce.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost("products")]
        public async Task<ActionResult<ServiceResponse<List<CartProductResponseDTO>>>> GetCartProductsAsync(List<CartItem> cartItems)
        {
            return Ok(await _cartService.GetCartProductsAsync(cartItems));
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<CartProductResponseDTO>>>> StoreCartItemsAsync(List<CartItem> cartItems)
        {
            return Ok(await _cartService.StoreCartItemsAsync(cartItems));
        }

        [HttpPost("add")]
        public async Task<ActionResult<ServiceResponse<bool>>> AddItemToCartAsync(CartItem cartItem)
        {
            return Ok(await _cartService.AddItemToCartAsync(cartItem));
        }

        [HttpPut("update-quantity")]
        public async Task<ActionResult<ServiceResponse<bool>>> UpdateItemsQuantityAsync(CartItem cartItem)
        {
            return Ok(await _cartService.UpdateItemsQuantityAsync(cartItem));
        }

        [HttpGet("count")]
        public async Task<ActionResult<ServiceResponse<int>>> GetCartItemsCountAsync()
        {
            return await _cartService.GetCartItemsCountAsync();
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<CartProductResponseDTO>>>> GetDbCartProductsAsync()
        {
            return Ok(await _cartService.GetDbCartProductsAsync());
        }


    }
}
