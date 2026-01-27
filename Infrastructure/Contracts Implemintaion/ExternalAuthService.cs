using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Application.Contracts;
using Application.DTOS.Auth_DTOS;

using Domain.Entites;

using Google.Apis.Auth;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Contracts_Implemintaion
{
    public class ExternalAuthService: IExternalAuthService
    {
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;
        private HttpClient _httpClient;

        public ExternalAuthService(IConfiguration config, IUnitOfWork unitOfWork, HttpClient httpClient)
        {
            _config = config;
            _unitOfWork = unitOfWork;
            _httpClient = httpClient;
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

        #region GoogleAuth
        //public async Task<ExternalAuthResponse> AuthGoogle(ExternalAuthDTO dto)
        //{
        //    ExternalAuthResponse DTO = new ExternalAuthResponse(IsAuthenticated: false, "Failed Registration");

        //    var settings = new GoogleJsonWebSignature.ValidationSettings()
        //    {
        //        Audience = new List<string> { _config["Google:ClientId"] }
        //    };

        //    var payload = await GoogleJsonWebSignature.ValidateAsync(dto.IdToken, settings);

        //    var user = await _unitOfWork.UserRepo.FindUserByEmail(payload.Email);

        //    if (user == null)
        //    {
        //        user = new User { Email = payload.Email, UserName = payload.Email, EmailConfirmed = true, };
        //        var result = await _unitOfWork.UserRepo.CreateAsync(user);
        //    }

        //    var myJwt = GenerateJwt(user);
        //    if (!string.IsNullOrEmpty(myJwt))
        //    {
        //        DTO = new ExternalAuthResponse(IsAuthenticated: false, myJwt);
        //    }
        //    return DTO;
        //}
        #endregion
        #region GithhubAuth
        public async Task<string> GetGitHubAccessToken(string code)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://github.com/login/oauth/access_token");
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = _config["GitHub:ClientId"],
                ["client_secret"] = _config["GitHub:ClientSecret"],
                ["code"] = code
            });
            request.Headers.Add("Accept", "application/json");

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadFromJsonAsync<JsonElement>();
            if (content.ValueKind == JsonValueKind.Undefined ||
                !content.TryGetProperty("access_token", out var tokenElement))
                return null;

            return tokenElement.GetString();
        }

        public async Task<LoginResponse> GitHubAuth(string code)
        {
            LoginResponse DTO = new LoginResponse(false, "invaid Token ");
            var githubToken = await GetGitHubAccessToken(code);

            if (string.IsNullOrEmpty(githubToken)) return DTO;

            var githubUser = await GetGitHubUserInfo(githubToken);

            var user = await _unitOfWork.UserRepo.FindUserByEmail(githubUser.Email);

            if (user == null)
            {
                user = new User
                {
                    UserName = githubUser.Email,
                    Email = githubUser.Email,
                    EmailConfirmed = true
                };

                var creating = await _unitOfWork.UserRepo.CreateAsync(user);
            }

            var token = GenerateJwt(user);
            DTO = new LoginResponse(true, token);
            return DTO;
        }

        public async Task<GitHubUserInfo> GetGitHubUserInfo(string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Invento-App");

            var profileResponse = await _httpClient.GetFromJsonAsync<JsonElement>("https://api.github.com/user");
            if (profileResponse.ValueKind == JsonValueKind.Undefined)
                throw new Exception("Invalid GitHub profile response.");
            var githubId = profileResponse.GetProperty("id").ToString();
            var name = profileResponse.GetProperty("name").GetString() ?? profileResponse.GetProperty("login").GetString();
            var avatar = profileResponse.GetProperty("avatar_url").GetString();

            var emailsResponse = await _httpClient.GetFromJsonAsync<List<GitHubEmailResponse>>("https://api.github.com/user/emails");
            if (emailsResponse == null || emailsResponse.Count == 0)
                throw new Exception("GitHub email list is empty.");
            var primaryEmail = emailsResponse?.FirstOrDefault(e => e.Primary && e.Verified)?.Email
                               ?? emailsResponse?.FirstOrDefault()?.Email;

            if (string.IsNullOrEmpty(primaryEmail))
                throw new Exception("Could not retrieve verified email from GitHub.");

            return new GitHubUserInfo(githubId, name!, primaryEmail, avatar!);
        }
        #endregion
    }
}
