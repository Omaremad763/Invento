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
using Infrastructure.Repos;

using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Application.Internal_Services_implementation
{
    public class ProductcService(IMapper mapper, IUnitOfWork unitOfWork) : IProductService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<bool>AddProductAsync(AddProductDto dto)
        {
            var Mapping= _mapper.Map<Product>(dto);
            await _unitOfWork.Products.AddAsync(Mapping);
            int saving= await _unitOfWork.CommitAsync();
            return saving >0;
        }

        public async Task<PaginatedResult<GetProductsDto>> GetAllProductsAsync(ResourceParameters parameters)
        {

            var products = _unitOfWork.Products.GetAllWithIncludeAsync(p => p.Category);

            if (parameters.CategoryId != null)
            {
                products = products.Where(p => p.CategoryId==parameters.CategoryId);

            }
             if (!string.IsNullOrEmpty(parameters.SearchTerm))
            {
                var search = parameters.SearchTerm.Trim().ToLower();
                products = products.Where(p => p.Name.ToLower().Contains(search)|| p.SKU.ToLower().Contains(search));
            }
            var projectedQuery = products.ProjectTo<GetProductsDto>(_mapper.ConfigurationProvider);

            var result= await projectedQuery.ToPaginatedListAsync(parameters.PageNumber, parameters.PageSize);
            return result;
        }

        public async Task<GetProductsDto?> GetProductByIdAsync(Guid id)
        {

            var products = await _unitOfWork.Products.GetByIdAsync(id);
            return _mapper.Map<GetProductsDto>(products);
        }

        public async Task<bool> UpdateProductAsync(UpdateProductDto dto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(dto.Id);
            if (product == null)
            {
                return false;
            }

            _mapper.Map(dto, product);
            int saving = await _unitOfWork.CommitAsync();
            //return true if the saving is greater than 0
            return saving > 0;
        }

        public async Task<bool> SoftDeleteProductAsync(Guid id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                return false;
            }
            product.IsDeleted = true;
            int saving = await _unitOfWork.CommitAsync();
            //return true if the saving is greater than 0
            return saving > 0;
        }
    }
}
