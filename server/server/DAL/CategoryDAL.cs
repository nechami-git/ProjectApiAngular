using Microsoft.EntityFrameworkCore;
using server.DAL.Interfaces;
using server.Models;

namespace server.DAL
{
    public class CategoryDAL : ICategoryDAL
    {
        private readonly ChineseSaleContext _dbContext;
        public CategoryDAL(ChineseSaleContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<List<CategoryModel>> GetAllCategories()
        {
            return await _dbContext.Categories.AsNoTracking().ToListAsync();
        }
        public async Task Post(CategoryModel category)
        {
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> IsCategoryExists(string name)
        {
            return await _dbContext.Categories.AnyAsync(c => c.Name!.Trim() == name.Trim());
        }
        public async Task<bool> HasGifts(int categoryId)
        {
            return await _dbContext.Gifts.AnyAsync(g => g.CategoryId == categoryId);
        }
        public async Task Delete(int id)
        {
            var category = await _dbContext.Categories.FindAsync(id);
            if (category != null)
            {
                _dbContext.Categories.Remove(category);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
