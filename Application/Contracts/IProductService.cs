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
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto?> GetProductByIdAsync(Guid id);
        Task<bool> AddProductAsync(ProductDto dto);
        Task<bool> UpdateProductAsync(ProductDto dto);

        Task<bool> SoftDeleteProductAsync(Guid id);
    }

}
