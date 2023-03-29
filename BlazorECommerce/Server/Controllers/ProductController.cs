﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

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
        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetProduct()
        {
            return Ok(await _productService.GetProductsAsync());
        }
        [HttpGet("{productId}")]
        public async Task<ActionResult<Product>> GetProduct(int productId)
        {
            return Ok(await _productService.GetProductAsync(productId));
        }
    }
}
