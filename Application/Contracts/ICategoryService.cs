using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.DTOS;

namespace Application.Contracts
{

    public interface ICategoryService
    {
        Task<PaginatedResult<CategoryDto>> GetAllCategoriesAsync(ResourceParameters parameters);
        Task<bool> UpdateCategoryAsync(CategoryDto dto);
        Task<bool> AddCategoryAsync(string CategoryName);
        Task<bool> SoftDeleteCategoryAsync(Guid id);
    }
}
