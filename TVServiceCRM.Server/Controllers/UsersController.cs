using Microsoft.AspNetCore.Mvc;
using TVServiceCRM.Server.Model.Models;
using TVServiceCRM.Server.Business.IServices;
using System.Linq.Expressions;
using TVServiceCRM.Server.Business;
using Microsoft.EntityFrameworkCore;
using TVServiceCRM.Server.Model.Dtos.DataTable;
using Microsoft.AspNetCore.Authorization;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using TVServiceCRM.Server.Model.Dtos;
using System.Security.Claims;
using TVServiceCRM.Server.Business.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Azure;
using Microsoft.AspNetCore.Identity.Data;
using System.Threading.Tasks;
using TVServiceCRM.Server.Model.System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace TVServiceCRM.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UsersController : ControllerBase
    {
        private readonly IDataService _dataService;
        private readonly ILogger<TicketsController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly TokenSettings _tokenSettings;

        public UsersController(
            IDataService dataService,
            ILogger<TicketsController> logger,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
             TokenSettings tokenSettings)
        {
            _dataService = dataService;
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _tokenSettings = tokenSettings;
        }


        [HttpPost]
        public async Task<ApiResponse<bool>> Register(UserRegisterRequestDto request)
        {
            var user = new ApplicationUser()
            {
                UserName = request.Email,
                Email = request.Email,

            };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                return new ApiResponse<bool>().SetSuccessResponse(true);
            }
            else
            {
                return new ApiResponse<bool>().SetErrorResponse(GetRegisterErrors(result));
            }


        }

        private Dictionary<string, string[]> GetRegisterErrors(IdentityResult result)
        {
            var errorDictionary = new Dictionary<string, string[]>(1);

            foreach (var error in result.Errors)
            {
                string[] newDescriptions;

                if (errorDictionary.TryGetValue(error.Code, out var descriptions))
                {
                    newDescriptions = new string[descriptions.Length + 1];
                    Array.Copy(descriptions, newDescriptions, descriptions.Length);
                    newDescriptions[descriptions.Length] = error.Description;
                }
                else
                {
                    newDescriptions = [error.Description];
                }

                errorDictionary[error.Code] = newDescriptions;
            }

            return errorDictionary;
        }


        [HttpPost]
        public async Task<ApiResponse<UserLoginResponseDto>> Login(UserLoginRequestDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {

                return new ApiResponse<UserLoginResponseDto>().SetErrorResponse("email", "Email not found");
            }
            else
            {
                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, true);
                if (result.Succeeded)
                {
                    var token = await GenerateUserToken(user);
                    return new ApiResponse<UserLoginResponseDto>().SetSuccessResponse(token);
                }
                else
                {
                    return new ApiResponse<UserLoginResponseDto>().SetErrorResponse("password", result.ToString());
                }
            }
        }

        [HttpPost]
        public async Task<ApiResponse<UserRefreshResponseDto>> refreshtoken(UserRefreshRequestDto request)
        {
            var principal = GetPrincipalFromExpiredToken(_tokenSettings, request.AccessToken);
            if (principal == null || principal.FindFirst("UserName")?.Value == null)
            {
                return new ApiResponse<UserRefreshResponseDto>().SetErrorResponse("email", "User not found");
            }
            else
            {
                var user = await _userManager.FindByNameAsync(principal.FindFirst("UserName")?.Value ?? "");
                if (user == null)
                {
                    return new ApiResponse<UserRefreshResponseDto>().SetErrorResponse("email", "User not found");
                }
                else
                {
                    if (!await _userManager.VerifyUserTokenAsync(user, "REFRESHTOKENPROVIDER", "RefreshToken", request.RefreshToken))
                    {
                        return new ApiResponse<UserRefreshResponseDto>().SetErrorResponse("token", "Refresh token expired");
                    }
                    var token = await GenerateUserToken(user);
                    return new ApiResponse<UserRefreshResponseDto>()
                        .SetSuccessResponse(new UserRefreshResponseDto() { AccessToken = token.AccessToken, RefreshToken = token.RefreshToken });
                }
            }
        }
        [HttpPost]
        public async Task<ApiResponse<bool>> logout()
        {

            if (User.Identity?.IsAuthenticated ?? false)
            {
                var username = User.Claims.First(x => x.Type == "UserName").Value;
                var appUser = await _dataService.Users.FirstOrDefaultAsync(x => x.UserName == username);
                if (appUser != null) { await _userManager.UpdateSecurityStampAsync(appUser); }
                return new ApiResponse<bool>().SetSuccessResponse(true);
            }
            return new ApiResponse<bool>().SetSuccessResponse(true);
        }

        [HttpPost]
        //[Authorize]
        [Authorize(Roles = "Administrator")]
        public string Profile()
        {
            return User.FindFirst("UserName")?.Value ?? "";
        }




        private async Task<UserLoginResponseDto> GenerateUserToken(ApplicationUser user)
        {
            var claims = (from ur in _dataService.Query.UserRoles
                          where ur.UserId == user.Id
                          join r in _dataService.Query.Roles on ur.RoleId equals r.Id
                          join rc in _dataService.Query.RoleClaims on r.Id equals rc.RoleId
                          select rc)
              .Where(rc => !string.IsNullOrEmpty(rc.ClaimValue) && !string.IsNullOrEmpty(rc.ClaimType))
              .Select(rc => new Claim(rc.ClaimType!, rc.ClaimValue!))
              .Distinct()
              .ToList();

            var roleClaims = (from ur in _dataService.Query.UserRoles
                              where ur.UserId == user.Id
                              join r in _dataService.Query.Roles on ur.RoleId equals r.Id
                              select r)
              .Where(r => !string.IsNullOrEmpty(r.Name))
              .Select(r => new Claim(ClaimTypes.Role, r.Name!))
              .Distinct()
              .ToList();

            claims.AddRange(roleClaims);

            var token = GetToken(_tokenSettings, user, claims);
            await _userManager.RemoveAuthenticationTokenAsync(user, "REFRESHTOKENPROVIDER", "RefreshToken");
            var refreshToken = await _userManager.GenerateUserTokenAsync(user, "REFRESHTOKENPROVIDER", "RefreshToken");
            await _userManager.SetAuthenticationTokenAsync(user, "REFRESHTOKENPROVIDER", "RefreshToken", refreshToken);
            return new UserLoginResponseDto() { AccessToken = token, RefreshToken = refreshToken };
        }

        private string GetToken(TokenSettings appSettings, ApplicationUser user, List<Claim> roleClaims)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.SecretKey));
            var signInCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var userClaims = new List<Claim>();
            userClaims.Add(new("Id", user.Id.ToString()));
            userClaims.Add(new("UserName", user.UserName ?? ""));
            userClaims.AddRange(roleClaims);
            userClaims.AddRange(appSettings.Audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));


            var tokeOptions = new JwtSecurityToken(
                issuer: appSettings.Issuer,
                audience: appSettings.Audience,
                claims: userClaims,
                expires: DateTime.UtcNow.AddSeconds(appSettings.TokenExpireSeconds),
                signingCredentials: signInCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

            return tokenString;
        }



        private ClaimsPrincipal GetPrincipalFromExpiredToken(TokenSettings appSettings, string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidAudiences = appSettings.Audiences,
                //ValidIssuer = appSettings.Issuer,
                ValidateLifetime = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.SecretKey))
            };

            var principal = new JwtSecurityTokenHandler().ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("GetPrincipalFromExpiredToken Token is not validated");

            return principal;
        }
    }
}
