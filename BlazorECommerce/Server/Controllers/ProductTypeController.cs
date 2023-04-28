using Microsoft.AspNetCore.Authorization;

namespace BlazorECommerce.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ProductTypeController : ControllerBase
    {
        private readonly IProductTypeService _productTypeService;

        public ProductTypeController(IProductTypeService productTypeService)
        {
            _productTypeService = productTypeService;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<ProductType>>>> GetProductTypesAsync()
        {
            return Ok(await _productTypeService.GetProductTypesAsync());
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<ProductType>>>> AddProductTypesAsync(ProductType productType)
        {
            return Ok(await _productTypeService.AddProductTypesAsync(productType));
        }

        [HttpPut]
        public async Task<ActionResult<ServiceResponse<List<ProductType>>>> UpdateProductTypesAsync(ProductType productType)
        {
            return Ok(await _productTypeService.UpdateProductTypesAsync(productType));
        }
    }
}
