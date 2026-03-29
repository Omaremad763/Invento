using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.Contracts;
using Application.DTOS;

using AutoMapper;
using AutoMapper.QueryableExtensions;

using Domain.Entites;

using Infrastructure.Extentions;

namespace Application.Internal_Services_implementation
{
    public class CategoryService(IMapper mapper, IUnitOfWork unitOfWork) : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<PaginatedResult<CategoryDto>> GetAllCategoriesAsync(ResourceParameters parameters)
        {
            IQueryable<Category>? Categories = _unitOfWork.Categories.GetAllAsync();

            if (parameters.CategoryId != null)
            {
                Categories = Categories.Where(p => p.Id == parameters.CategoryId);

            }
            if (!string.IsNullOrEmpty(parameters.SearchTerm))
            {
                var search = parameters.SearchTerm.Trim().ToLower();
                Categories = Categories.Where(p => p.CategoryName.Contains(search, StringComparison.CurrentCultureIgnoreCase));
            }

            var projectedQuery = Categories.ProjectTo<CategoryDto>(_mapper.ConfigurationProvider);

            var result = await projectedQuery.ToPaginatedListAsync(parameters.PageNumber, parameters.PageSize);
            return result;
        }


        public async Task<bool> AddCategoryAsync(string CategoryName)
        {
            var Newcategory = new Category(CategoryName);

            await _unitOfWork.Categories.AddAsync(Newcategory);
            int saving = await _unitOfWork.CommitAsync();
            return saving > 0;
        }

        public async Task<bool> UpdateCategoryAsync(CategoryDto dto)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(dto.Id);
            if (category == null)
            {
                return false;
            }
            _mapper.Map(dto, category);
            int saving = await _unitOfWork.CommitAsync();
            //return true if the saving is greater than 0
            return saving > 0;
        }

        public async Task<bool> SoftDeleteCategoryAsync(Guid id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
            {
                return false;
            }
            var products = _unitOfWork.Products.GetAllWithIncludeAsync(p => p.Category).Where(p => p.CategoryId == id);
            foreach (var product in products) { product.IsDeleted = true; }
            category.IsDeleted = true;
            int saving = await _unitOfWork.CommitAsync();
            //return true if the saving is greater than 0
            return saving > 0;
        }


    }
}
