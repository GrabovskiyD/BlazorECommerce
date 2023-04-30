using Microsoft.AspNetCore.Authorization;

namespace BlazorECommerce.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("admin"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<ServiceResponse<List<Product>>>> GetAdminProductAsync()
        {
            return Ok(await _productService.GetAdminProductsAsync());
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<Product>>>> GetProduct()
        {
            return Ok(await _productService.GetProductsAsync());
        }

        [HttpGet("{productId}")]
        public async Task<ActionResult<ServiceResponse<Product>>> GetProduct(int productId)
        {
            return Ok(await _productService.GetProductAsync(productId));
        }

        [HttpGet("category/{categoryUrl}")]
        public async Task<ActionResult<ServiceResponse<List<Product>>>> GetProductsByCategory(string categoryUrl)
        {
            return Ok(await _productService.GetProductsByCategoryAsync(categoryUrl));
        }

        [HttpGet("search/{searchText}/{page}")]
        public async Task<ActionResult<ServiceResponse<ProductSearchResultDTO>>> SearchProducts(string searchText, int page = 1)
        {
            return Ok(await _productService.SearchProductsAsync(searchText, page));
        }

        [HttpGet("searchSuggestions/{searchText}")]
        public async Task<ActionResult<ServiceResponse<List<Product>>>> GetProductSearchSuggestions(string searchText)
        {
            return Ok(await _productService.GetProductSearchSuggestionsAsync(searchText));
        }

        [HttpGet("featured")]
        public async Task<ActionResult<ServiceResponse<List<Product>>>> GetFeaturedProducts()
        {
            return Ok(await _productService.GetFeaturedProductsAsync());
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult<ServiceResponse<Product>>> CreateProductAsync(Product product)
        {
            return Ok(await _productService.CreateProductAsync(product));
        }

        [HttpPut, Authorize(Roles = "Admin")]
        public async Task<ActionResult<ServiceResponse<Product>>> UpdateProductAsync(Product product)
        {
            return Ok(await _productService.UpdateProductAsync(product));
        }

        [HttpDelete("{id}"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<ServiceResponse<bool>>> DeleteProductAsync(int id)
        {
            return Ok(await _productService.DeleteProductAsync(id));
        }
    }
}
