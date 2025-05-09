// using Infrastructure.Contexts;
// using Infrastructure.Repositories.Interfaces;
// using Microsoft.EntityFrameworkCore;
//
// namespace Infrastructure.Repositories
// {
// 	public class CategoryRepository(DocumentDbContext context) : BaseRepository<Category>(context), ICategoryRepository
// 	{
// 		public async Task<bool> AddCategory(Category category)
// 		{
// 			var result = await context.Database.ExecuteSqlRawAsync(
// 				"INSERT INTO \"Category\" (Id, \"categoryName\", \"categorySlug\") VALUES ({0}, {1}, {2})",
// 				category.Id, category.CategoryName, category.CategorySlug
// 			);
// 			return result > 0;
// 		}
//
// 		public async Task<bool> UpdateCategory(Category category)
// 		{
// 			var result = await context.Database.ExecuteSqlRawAsync(
// 				"UPDATE \"Category\" SET \"categoryName\" = {1}, \"categorySlug\" = {2} WHERE Id = {0}",
// 				category.Id, category.CategoryName, category.CategorySlug
// 			);
// 			return result > 0;
// 		}
//
// 		public async Task<bool> DeleteCategory(Guid id)
// 		{
// 			var result = await context.Database.ExecuteSqlRawAsync(
// 				"DELETE FROM \"Category\" WHERE Id = {0}", id
// 			);
// 			return result > 0;
// 		}
//
// 		public async Task<Category> GetCategoryById(Guid id)
// 		{
// 			return await context.Categories
// 				.FromSqlRaw("SELECT * FROM \"Category\" WHERE Id = {0}", id)
// 				.FirstOrDefaultAsync();
// 		}
//
// 		public async Task<List<Category>> GetCategories()
// 		{
// 			return await context.Categories
// 				.FromSqlRaw("SELECT * FROM \"Category\"")
// 				.ToListAsync();
// 		}
// 	}
// }
