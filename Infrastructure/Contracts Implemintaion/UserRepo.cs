using Application.Contracts;

using Domain.Entites;

using Infrastructure.Persistence;

using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Contracts_Implemintaion
{
    public class UserRepo(ApplicationDbContext context, 
        UserManager<User> userManager,
       SignInManager<User> signInManager  
        ) : IUserRepo
    {
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<User> _userManager = userManager;
        private readonly SignInManager<User> _signInManager = signInManager;

        public async Task<User?> GetUserData(Guid Id)
        {
            var UserData = await _context.Users.FindAsync(Id);
            return UserData ?? null;
        }

        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            var isValid = await _userManager.CheckPasswordAsync(user, password);
            return isValid;
        }

        public async Task<IdentityResult> CreateAsync(User user, string? password = null)
        {
            IdentityResult isValid;
            //Create Mail By Google
            if (string.IsNullOrEmpty(password))
            {
                isValid = await _userManager.CreateAsync(user);
            }
            //Create Mail By the traditional way
            else
            {
                isValid = await _userManager.CreateAsync(user, password);
            }
            return isValid;
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            return token;
        }

        public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
        {
            return await _userManager.ConfirmEmailAsync(user, token);
        }

        public async Task<User?> FindUserByEmail(string Email)
        {
            var UserData = await _userManager.FindByEmailAsync(Email);
            return UserData ?? null;
        }

        public async Task<SignInResult> CheckSigninManagerAsync(User userEntity, string password, bool lockoutOnFailure)
        {
          return await _signInManager.CheckPasswordSignInAsync(userEntity, password, lockoutOnFailure);
        }
    }
}