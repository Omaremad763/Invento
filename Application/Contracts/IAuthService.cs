using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.DTOS.Auth_DTOS;

using Domain.Entites;

namespace Application.Contracts
{
    public interface IAuthService
    {
        Task<RegisterResponse> RegisterAsync(RegisterDto request);
        Task<LoginResponse> LoginAsync(LoginDto request);
        Task<ConfirmResponse> ConfirmEmailAsync(ConfirmEmailDto DTO);
    }
}
