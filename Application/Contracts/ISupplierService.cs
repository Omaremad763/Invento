using Application.DTOS;

namespace Application.Contracts
{
    public interface ISupplierService
    {
        Task<PaginatedResult<SupplierDto>> GetAllSuppliersAsync(ResourceParameters Parameters);
        Task<bool> UpdateSupplierAsync(UpdateSupplierDto dto);
        Task<(bool, string)> AddSupplierAsync(AddSupplierDto DTO, string CountryCode, string VatNumber);
        Task<bool> SoftDeleteSupplierAsync(Guid id);
    }
}
