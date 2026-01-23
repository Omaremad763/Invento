using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.DTOS;

namespace Application.Contracts
{
    public interface IExternalApisService
    {
        Task<(bool IsValid, string CompanyName)> ValidateVatAsync(string countryCode, string vatNumber);
    }
}
