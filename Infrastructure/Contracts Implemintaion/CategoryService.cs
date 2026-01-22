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
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<PaginatedResult<CategoryDto>> GetAllCategoriesAsync(ResourceParameters parameters)
        {
            var Categories = _unitOfWork.Categories.GetAllAsync();
            var projectedQuery = Categories.ProjectTo<CategoryDto>(_mapper.ConfigurationProvider);

            var result = await projectedQuery.ToPaginatedListAsync(parameters.PageNumber, parameters.PageSize);
            return result;
        }
    }
}
