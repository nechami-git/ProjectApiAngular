using server.Models;
using server.Models.DTO;

namespace server.BLL.Intarfaces
{
    public interface ICategoryBLL
    {
        Task<List<CategoryDTO>> GetAllCategories();
        Task Post(CategoryDTO category);
        Task Delete(int id);
    }
}
