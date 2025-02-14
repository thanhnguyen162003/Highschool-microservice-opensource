using Infrastructure.Contexts;
using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Repositories
{
    public class CategoryRepository(DocumentDbContext context) : BaseRepository<Category>(context), ICategoryRepository
    {
        public async Task<bool> AddCategory(Category category)
        {
            await _entities.AddAsync(category);
            var result = await context.SaveChangesAsync();
            if (result > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateCategory(Category category)
        {
            _entities.Update(category);
            var result = await context.SaveChangesAsync();
            if (result > 0)
            {
                return true;    
            }
            return false;
        }

        public async Task<bool> DeleteCategory(Guid id)
        {
            var category = await _entities.FindAsync(id);
            if (category != null)
            {
                _entities.Remove(category);
            }
            var result = await context.SaveChangesAsync();
            if (result > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<Category> GetCategoryById(Guid id)
        {
            return await _entities.FindAsync(id);
        }

        public async Task<List<Category>> GetCategories()
        {
            return await _entities.ToListAsync();
        }
    }
}
