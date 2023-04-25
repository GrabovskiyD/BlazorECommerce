namespace BlazorECommerce.Server.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly DataContext _dataContext;

        public CategoryService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<ServiceResponse<List<Category>>> AddCategoryAsync(Category category)
        {
            category.Editing = category.IsNew = false;
            _dataContext.Categories.Add(category);
            await _dataContext.SaveChangesAsync();
            return await GetAdminCategoriesAsync();

        }

        public async Task<ServiceResponse<List<Category>>> DeleteCategoryAsync(int id)
        {
            Category category = await GetCategoryByIdAsync(id);
            if(category is null)
            {
                return new ServiceResponse<List<Category>>
                {
                    Success = false,
                    Message = "Category not found."
                };
            }
            category.Deleted = true;
            await _dataContext.SaveChangesAsync();
            return await GetAdminCategoriesAsync();
        }

        private async Task<Category> GetCategoryByIdAsync(int id)
        {
            return await _dataContext.Categories.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<ServiceResponse<List<Category>>> GetAdminCategoriesAsync()
        {
            var categories = await _dataContext.Categories
                .Where(c => !c.Deleted)
                .ToListAsync();
            return new ServiceResponse<List<Category>>
            {
                Data = categories,
                Success = true
            };
        }

        public async Task<ServiceResponse<List<Category>>> GetCategoriesAsync()
        {
            var categories = await _dataContext.Categories
                .Where(c => !c.Deleted && c.Visible)
                .ToListAsync();
            return new ServiceResponse<List<Category>>
            {
                Data = categories,
                Success = true
            };
        }

        public async Task<ServiceResponse<List<Category>>> UpdateCategoryAsync(Category category)
        {
            var dbCategory = await GetCategoryByIdAsync(category.Id);
            if(dbCategory is null)
            {
                return new ServiceResponse<List<Category>>
                {
                    Success = false,
                    Message = "Category not found."
                };
            }
            dbCategory.Name = category.Name;
            dbCategory.Url = category.Url;
            dbCategory.Visible = category.Visible;

            await _dataContext.SaveChangesAsync();
            return await GetAdminCategoriesAsync();
        }
    }
}
