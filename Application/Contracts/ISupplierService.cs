using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.DTOS;
using Application.DTOS.Update_DTOS;

namespace Application.Contracts
{
    public interface ISupplierService
    {
        Task<PaginatedResult<SupplierDto>> GetAllSuppliersAsync(ResourceParameters Parameters);
        Task<bool> UpdateSupplierAsync(UpdateSupplierDTO dto);
        Task<(bool, string)> AddSupplierAsync(AddSupplierDTO DTO, string CountryCode, string VatNumber);
        Task<bool> SoftDeleteSupplierAsync(Guid id);
    }
}
