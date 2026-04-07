using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;

using Application.Contracts;
using Application.DTOS.Auth_DTOS;

using Domain.Entites;

using FluentEmail.Core;

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
namespace authservcie;
public class AuthService(IConfiguration config, IUnitOfWork unitOfWork, IFluentEmail email,
    IMemoryCache memoryCache) : IAuthService
{
    private readonly IConfiguration _config = config;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IFluentEmail _email = email;
    private readonly IMemoryCache _memoryCache = memoryCache;
    private readonly Assembly _assembly = typeof(AuthService).Assembly;
    private const string ConfirmEmailTemplateCacheKey = "email:template:confirm";

    private async Task<string> GetConfirmEmailTemplateAsync()
    {
        if (_memoryCache.TryGetValue(ConfirmEmailTemplateCacheKey, out string cachedTemplate))
            return cachedTemplate;

        var resourceName = "Infrastructure.RazorPages.ConfirmEmail.html";
        await using var stream = _assembly.GetManifestResourceStream(resourceName)
            ?? throw new FileNotFoundException($"Embedded resource {resourceName} not found.");

        using var reader = new StreamReader(stream);

        var html = await reader.ReadToEndAsync();

        _memoryCache.Set(
            ConfirmEmailTemplateCacheKey,
            html,
            new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6),
                Priority = CacheItemPriority.High
            });

        return html;
    }

    private async Task<bool> SendEmailAsync(string to, string subject, SendEmailDto dto)
    {
        bool Success = false;

        string html = await GetConfirmEmailTemplateAsync();
        html = html.Replace("{{UserName}}", dto.UserName)
               .Replace("{{ConfirmLink}}", dto.Link);

        var mail = await _email.To(to).Subject(subject)
            .Body(html, isHtml: true)
            .SendAsync();
        if (mail.Successful) { Success = true; }
        return Success;
    }
    private string GenerateJwt(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email!)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterDto request)
    {
        var _frontendUrl = _config["Frontend:BaseUrl"];
        RegisterResponse DTO = new(IsAuthenticated: false, "Failed Registration");
        var user = new User { UserName = request.UserName, Email = request.Email };

        Microsoft.AspNetCore.Identity.IdentityResult? result = await _unitOfWork.UserRepo.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
            DTO = new RegisterResponse(false, errorMessages);
            return DTO;
        }

        var token = await _unitOfWork.UserRepo.GenerateEmailConfirmationTokenAsync(user);

        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        var link = $"{_frontendUrl}/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(encodedToken)}";

        SendEmailDto model = new(UserName: user.UserName, link);

        var sending = await SendEmailAsync(user.Email, "Confirm your email", model);

        if (sending)
        {
            DTO = new RegisterResponse(true, "Check your email to confirm");
        }
        return DTO;
    }

    public async Task<LoginResponse> LoginAsync(LoginDto request)
    {
        LoginResponse DTO = new(false, "Invalid email or password");
        var user = await _unitOfWork.UserRepo.FindUserByEmail(request.Email);
        if (string.IsNullOrEmpty(request.Password)) return DTO;
        var isValid = await _unitOfWork.UserRepo.CheckPasswordAsync(user, request.Password);

        if (user == null || !isValid) return DTO;

        if (!user.EmailConfirmed)
            return new LoginResponse(false, "Email not confirmed");

        var token = GenerateJwt(user);

        return new LoginResponse(true, token);
    }

    public async Task<ConfirmResponse> ConfirmEmailAsync(ConfirmEmailDto DTO)
    {
        var user = await _unitOfWork.UserRepo.GetUserData(DTO.UserID);
        if (user == null)
            return new ConfirmResponse(false, "User not found");

        var decodedTokenBytes = WebEncoders.Base64UrlDecode(DTO.Token);
        var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

        var result = await _unitOfWork.UserRepo.ConfirmEmailAsync(user, decodedToken);

        if (result.Succeeded) return new ConfirmResponse(true, "Email confirmed successfully!");

        return new ConfirmResponse(false, "Invalid or expired token");
    }
}