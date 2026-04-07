using Application.DTOS.Auth_DTOS;

namespace Application.Contracts
{
    public interface IAuthService
    {
        Task<RegisterResponse> RegisterAsync(RegisterDto request);

        Task<LoginResponse> LoginAsync(LoginDto request);

        Task<ConfirmResponse> ConfirmEmailAsync(ConfirmEmailDto DTO);
    }
}