using Microsoft.AspNetCore.Authorization;

namespace BlazorECommerce.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ProductTypes : ControllerBase
    {
        private readonly IProductTypeService _productTypeService;

        public ProductTypes(IProductTypeService productTypeService)
        {
            _productTypeService = productTypeService;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<ProductType>>>> GetProductTypesAsync()
        {
            return Ok(await _productTypeService.GetProductTypesAsync());
        }
    }
}
