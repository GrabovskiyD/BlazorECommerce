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
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Ok(await _cartService.StoreCartItemsAsync(cartItems, userId));
        }
    }
}
