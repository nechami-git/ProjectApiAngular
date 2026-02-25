using AutoMapper;
using server.BLL.Intarfaces;
using server.DAL;
using server.DAL.Interfaces;
using server.Exceptions;
using server.Models;
using server.Models.DTO;

namespace server.BLL
{
    public class CategoryBLL : ICategoryBLL
    {
        private readonly ICategoryDAL _categoryDAL;
        private readonly IMapper _mapper;
        public CategoryBLL(ICategoryDAL categoryDAL,IMapper mapper)
        {
            this._categoryDAL = categoryDAL;
            this._mapper = mapper;
        }
        public async Task<List<CategoryDTO>> GetAllCategories()
        {
            var categories = await _categoryDAL.GetAllCategories();
            return _mapper.Map<List<CategoryDTO>>(categories);
        }
        public async Task Post(CategoryDTO categoryDto)
        {
            if (categoryDto == null || string.IsNullOrWhiteSpace(categoryDto.Name))
            {
                throw new ArgumentException("שם קטגוריה לא יכול להיות ריק");
            }
            if (await _categoryDAL.IsCategoryExists(categoryDto.Name))
            {
                throw new ArgumentException("קטגוריה עם שם זה כבר קיימת");
            }
            var categoryEntity = _mapper.Map<CategoryModel>(categoryDto);
            await _categoryDAL.Post(categoryEntity);
        }
        public async Task Delete(int id)
        {

            if (id <= 0)
            {
                throw new BusinessException("קטגוריה לא חוקית למחיקה");
            }

            bool hasGifts = await _categoryDAL.HasGifts(id);
            if (hasGifts)
            {
                // return 409 Conflict via middleware
                throw new ConflictException("לא ניתן למחוק קטגוריה שיש לה מתנות משוייכות");
            }

            await _categoryDAL.Delete(id);
        }
    }
}