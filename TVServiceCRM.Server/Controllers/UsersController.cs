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

        public UsersController(
            IDataService dataService,
            ILogger<TicketsController> logger,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _dataService = dataService;
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
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
                    Response.Cookies.Append("AuthToken3", token.AccessToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = false, // Only sent over HTTPS
                        SameSite = SameSiteMode.Lax, // Prevent CSRF attacks
                        Domain ="localhost",
                        Expires = DateTimeOffset.UtcNow.AddHours(10), // Set expiration
                        IsEssential = true,
                        Path = "/"
                    });

                    Response.Cookies.Append("AuthToken2", token.AccessToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Domain ="localhost",
                IsEssential = true
                    });

                    return new ApiResponse<UserLoginResponseDto>().SetSuccessResponse(token);
                }
                else
                {
                    return new ApiResponse<UserLoginResponseDto>().SetErrorResponse("password", result.ToString());
                }
            }
        }

        //[HttpPost]
        //public async Task<AppResponse<UserRefreshTokenResponse>> RefreshToken(UserRefreshTokenRequest req)
        //{
        //    return await _userService.UserRefreshTokenAsync(req);
        //}
        //[HttpPost]
        //public async Task<AppResponse<bool>> Logout()
        //{
        //    return await _userService.UserLogoutAsync(User);
        //}

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

            var token = GetToken2(user, claims);
            //await _userManager.RemoveAuthenticationTokenAsync(user, "REFRESHTOKENPROVIDER", "RefreshToken");
            //var refreshToken = await _userManager.GenerateUserTokenAsync(user, "REFRESHTOKENPROVIDER", "RefreshToken");
            //await _userManager.SetAuthenticationTokenAsync(user, "REFRESHTOKENPROVIDER", "RefreshToken", refreshToken);
            return new UserLoginResponseDto() { AccessToken = token, RefreshToken = ""};
            //return new UserLoginResponseDto() { AccessToken = token, RefreshToken = refreshToken };
        }

        private string GetToken(ApplicationUser user, List<Claim> roleClaims)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("DFDGERsjsfjepoeoe@@#$$@$@123112sdaaadasQEWw"));
            var signInCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var userClaims = new List<Claim>
            {
                new("Id", user.Id.ToString()),
                new ("UserName", user.UserName??"")
            };
            userClaims.AddRange(roleClaims);
            var tokeOptions = new JwtSecurityToken(
                //issuer: appSettings.Issuer,
                //audience: appSettings.Audience,
                claims: userClaims,
                expires: DateTime.UtcNow.AddHours(10),
                signingCredentials: signInCredentials
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
            return tokenString;
        }


        private string GetToken2(ApplicationUser user, List<Claim> roleClaims)
        {
            // Validate user credentials (e.g., check username and password)

            // Generate JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("DFDGERsjsfjepoeoe@@#$$@$@123112sdaaadasQEWw"); // Use a secure key
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(roleClaims),
                
                Expires = DateTime.UtcNow.AddMinutes(30),
                //Issuer = "your_issuer",
                //Audience = "https://localhost:5173",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Set the JWT in an HttpOnly cookie
            Response.Cookies.Append("1AuthToken", tokenString, new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // Ensure this is true in production (HTTPS only)
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                IsEssential = true
            });


            // Set the JWT in an HttpOnly cookie
            Response.Cookies.Append("1AuthToken", tokenString, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Ensure this is true in production (HTTPS only)
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                IsEssential = true
            });


            Response.Cookies.Append("AuthToken", tokenString, new CookieOptions
            {
                HttpOnly = true,
                IsEssential = true
            });


            return tokenString ;
        }

    }
}
