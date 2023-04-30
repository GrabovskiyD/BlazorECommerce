namespace BlazorECommerce.Server.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly DataContext _dataContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductService(DataContext dataContext, IHttpContextAccessor httpContextAccessor)
        {
            _dataContext = dataContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ServiceResponse<Product>> GetProductAsync(int productId)
        {
            var response = new ServiceResponse<Product>();
            Product product = null;

            if (_httpContextAccessor.HttpContext.User.IsInRole("Admin"))
            {
                product = await _dataContext.Products
                    .Include(p => p.Variants.Where(v => !v.Deleted))
                    .ThenInclude(v => v.ProductType)
                    .FirstOrDefaultAsync(p => p.Id == productId && !p.Deleted);
            }
            else
            {
                product = await _dataContext.Products
                    .Include(p => p.Variants.Where(v => v.Visible && !v.Deleted))
                    .ThenInclude(v => v.ProductType)
                    .FirstOrDefaultAsync(p => p.Id == productId && p.Visible && !p.Deleted);
            }
            
            if (product == null)
            {
                response.Success = false;
                response.Message = "Sorry, but this product does not exist.";
            }
            else
            {
                response.Success = true;
                response.Data = product;
            }
            return response;
        }

        public async Task<ServiceResponse<List<Product>>> GetProductsAsync()
        {
            var response = new ServiceResponse<List<Product>>()
            {
                Data = await _dataContext.Products
                .Where(p => p.Visible && !p.Deleted)
                .Include(p => p.Variants.Where(v => v.Visible && !v.Deleted))
                .ToListAsync(),
                Success = true
            };
            return response;
        }

        public async Task<ServiceResponse<List<Product>>> GetProductsByCategoryAsync(string categoryUrl)
        {
            var response = new ServiceResponse<List<Product>>()
            {
                Data = await _dataContext.Products
                .Where(p => p.Category.Url.ToLower().Equals(categoryUrl.ToLower()) &&
                    p.Visible && !p.Deleted)
                .Include(p => p.Variants.Where(v => v.Visible && !v.Deleted))
                .ToListAsync(),
                Success = true
            };
            return response;
        }

        public async Task<ServiceResponse<ProductSearchResultDTO>> SearchProductsAsync(string searchText, int page)
        {
            var pageResults = 2f;
            var pageCount = Math.Ceiling((await FindProductsBySearchText(searchText)).Count / pageResults);
            var products = await _dataContext.Products
                            .Where(p => p.Title.ToLower().Contains(searchText.ToLower()) ||
                                p.Description.ToLower().Contains(searchText.ToLower()) &&
                                p.Visible && !p.Deleted)
                            .Include(p => p.Variants)
                            .Skip((page - 1) * (int)pageResults)
                            .Take((int)pageResults)
                            .ToListAsync();

            var response = new ServiceResponse<ProductSearchResultDTO>
            {
                Data = new ProductSearchResultDTO
                {
                    Products = products,
                    CurrentPage = page,
                    Pages = (int)pageCount
                }
            };
            if (response.Data is not null)
            {
                response.Success = true;
            }
            return response;
        }

        private async Task<List<Product>> FindProductsBySearchText(string searchText)
        {
            return await _dataContext.Products
                .Where(p => p.Title.ToLower().Contains(searchText.ToLower()) ||
                    p.Description.ToLower().Contains(searchText.ToLower()) &&
                    p.Visible && !p.Deleted)
                 .Include(p => p.Variants)
                 .ToListAsync();
        }

        public async Task<ServiceResponse<List<string>>> GetProductSearchSuggestionsAsync(string searchText)
        {
            var products = await FindProductsBySearchText(searchText);

            List<string> result = new List<string>();

            foreach (var product in products)
            {
                if (product.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(product.Title);
                }

                if (product.Description is not null)
                {
                    var punctuation = product.Description.Where(char.IsPunctuation)
                        .Distinct().ToArray();
                    var words = product.Description.Split()
                        .Select(s => s.Trim(punctuation));
                    foreach (string word in words)
                    {
                        if (word.Contains(searchText, StringComparison.OrdinalIgnoreCase) &&
                            !result.Contains(word))
                        {
                            result.Add(word);
                        }
                    }
                }
            }

            return new ServiceResponse<List<string>> { Data = result };
        }

        public async Task<ServiceResponse<List<Product>>> GetFeaturedProductsAsync()
        {
            var response = new ServiceResponse<List<Product>>()
            {
                Data = await _dataContext.Products
                .Where(p => p.Featured && p.Visible && !p.Deleted)
                .Include(p => p.Variants.Where(v => v.Visible && !v.Deleted))
                .ToListAsync()
            };

            return response;
        }

        public async Task<ServiceResponse<List<Product>>> GetAdminProductsAsync()
        {
            var response = new ServiceResponse<List<Product>>
            {
                Data = await _dataContext.Products
                    .Where(p => !p.Deleted)
                    .Include(p => p.Variants.Where(v => !v.Deleted))
                    .ThenInclude(v => v.ProductType)
                    .ToListAsync()
            };

            return response;
        }

        public async Task<ServiceResponse<Product>> CreateProductAsync(Product product)
        {
            foreach(var variant in product.Variants)
            {
                variant.ProductType = null;
            }
            _dataContext.Products.Add(product);
            await _dataContext.SaveChangesAsync();
            return new ServiceResponse<Product>
            {
                Data = product,
                Success = true
            };
        }

        public async Task<ServiceResponse<Product>> UpdateProductAsync(Product product)
        {
            var dbProduct = await _dataContext.Products.FindAsync(product.Id);
            if (dbProduct is null)
            {
                return new ServiceResponse<Product>
                {
                    Success = false,
                    Message = "Product not found."
                };
            }

            dbProduct.Title = product.Title;
            dbProduct.Description = product.Description;
            dbProduct.ImageUrl = product.ImageUrl;
            dbProduct.CategoryId = product.CategoryId;
            dbProduct.Visible = product.Visible;

            foreach(var variant in product.Variants)
            {
                var dbVariant = await _dataContext
                    .ProductVariants
                    .SingleOrDefaultAsync(v => v.ProductId == variant.ProductId &&
                    v.ProductTypeId == variant.ProductTypeId);
                if(dbVariant is null)
                {
                    variant.ProductType = null;
                    _dataContext.ProductVariants.Add(variant);
                }
                else
                {
                    dbVariant.ProductTypeId = variant.ProductTypeId;
                    dbVariant.Price = variant.Price;
                    dbVariant.OriginalPrice = variant.OriginalPrice;
                    dbVariant.Visible = variant.Visible;
                    dbVariant.Deleted = variant.Deleted;
                }
            }

            await _dataContext.SaveChangesAsync();
            return new ServiceResponse<Product>
            {
                Data = product
            };
        }

        public async Task<ServiceResponse<bool>> DeleteProductAsync(int productId)
        {
            var dbProduct = await _dataContext.Products.FindAsync(productId);
            if(dbProduct is null)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Success = false,
                    Message = "Product not found."
                };
            }

            dbProduct.Deleted = true;
            await _dataContext.SaveChangesAsync();
            return new ServiceResponse<bool> 
            {
                Data = true, 
                Success = true 
            };  
        }
    }
}
