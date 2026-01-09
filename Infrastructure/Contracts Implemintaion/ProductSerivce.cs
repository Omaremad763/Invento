using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.Contracts;
using Application.DTOS;

using AutoMapper;

using Domain.Entites;

using Infrastructure.Repos;

namespace Application.Internal_Services_implementation
{
    public class ProductcService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductcService( IMapper mapper,IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool>AddProductAsync(ProductDto dto)
        {
            var Mapping= _mapper.Map<Product>(dto);
            await _unitOfWork.Products.AddAsync(Mapping);
            int saving= await _unitOfWork.CommitAsync();
            //return true if saving more than 0
            return saving >0;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto?> GetProductByIdAsync(Guid id)
        {
            var products = await _unitOfWork.Products.GetByIdAsync(id);
            return _mapper.Map<ProductDto>(products);
        }

        public async Task<bool> UpdateProductAsync(ProductDto dto)
        {
            var Mapping = _mapper.Map<Product>(dto);
             _unitOfWork.Products.Update(Mapping);
            int saving = await _unitOfWork.CommitAsync();
            //return true if saving more than 0
            return saving > 0;
        }

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                return false; // Product not found
            }
            _unitOfWork.Products.Remove(product);
            int saving = await _unitOfWork.CommitAsync();
            return saving > 0;
        }
    }
}
