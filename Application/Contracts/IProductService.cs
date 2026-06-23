using Application.DTOS;

namespace Application.Contracts
{
    public interface IProductService
    {
        Task<PaginatedResult<GetProductsDto>> GetAllProductsAsync(ResourceParameters parameters);

        Task<GetProductsDto?> GetProductByIdAsync(Guid id);

        Task<bool> AddProductAsync(AddProductDto dto);

        Task<bool> UpdateProductAsync(UpdateProductDto dto);

        Task<bool> SoftDeleteProductAsync(Guid id);
    }
}