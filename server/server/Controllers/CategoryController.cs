using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.BLL.Intarfaces;
using server.Models.DTO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace server.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryBLL _categoryBLL;
        private readonly ILogger<CategoryController> _logger; // הזרקת הלוגר

        public CategoryController(ICategoryBLL categoryBLL, ILogger<CategoryController> logger)
        {
            _categoryBLL = categoryBLL;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<CategoryDTO>>> GetAll()
        {
            _logger.LogInformation("Attempting to retrieve all categories.");
            var categories = await _categoryBLL.GetAllCategories();
            return Ok(categories);
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult> Add(CategoryDTO categoryDto)
        {

            _logger.LogInformation("Manager is attempting to add a new category: {CategoryName}", categoryDto.Name);
            await _categoryBLL.Post(categoryDto);
            _logger.LogInformation("Category {CategoryName} added successfully.", categoryDto.Name);
            return Ok(new { message = "קטגוריה נוספה בהצלחה" });
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult> Delete(int id) 
        {
            _logger.LogInformation("Manager is attempting to delete category with ID: {CategoryId}", id);

            await _categoryBLL.Delete(id);

            _logger.LogInformation("Category with ID {CategoryId} deleted successfully.", id);
            return Ok(new { message = "קטגוריה נמחקה בהצלחה" });
        }
    }
}
