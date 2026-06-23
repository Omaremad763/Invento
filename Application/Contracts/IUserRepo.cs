using Domain.Entites;

using Microsoft.AspNetCore.Identity;

namespace Application.Contracts
{
    public interface IUserRepo
    {
        Task<User?> GetUserData(Guid Id);

        Task<bool> CheckPasswordAsync(User user, string password);

        Task<IdentityResult> CreateAsync(User user, string? password = null);

        Task<string> GenerateEmailConfirmationTokenAsync(User user);

        Task<IdentityResult> ConfirmEmailAsync(User user, string token);

        Task<User?> FindUserByEmail(string Email);

        Task<SignInResult> CheckSigninManagerAsync(User userEntity,string password, bool lockoutOnFailure);
    }
}