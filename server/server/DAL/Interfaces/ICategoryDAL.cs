using server.Models;

namespace server.DAL.Interfaces
{
    public interface ICategoryDAL
    {
        Task<List<CategoryModel>> GetAllCategories();
        Task Post(CategoryModel category);
        Task Delete(int id);
        Task<bool> IsCategoryExists(string name);
        Task<bool> HasGifts(int categoryId);
    }
}
