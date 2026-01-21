using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.DTOS;

namespace Application.Contracts
{
    public interface IProductService
    {
        Task<PaginatedResult<GetProductsDTO>> GetAllProductsAsync(ProductResourceParameters parameters);
        Task<GetProductsDTO?> GetProductByIdAsync(Guid id);
        Task<bool> AddProductAsync(AddProductDto dto);
        Task<bool> UpdateProductAsync(UpdateProductDto dto);

        Task<bool> SoftDeleteProductAsync(Guid id);
    }

}
