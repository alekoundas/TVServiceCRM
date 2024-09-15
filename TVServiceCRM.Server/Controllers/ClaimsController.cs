using Microsoft.AspNetCore.Mvc;
using TVServiceCRM.Server.Model.Models;
using TVServiceCRM.Server.Business.IServices;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using TVServiceCRM.Server.Model.System;
using TVServiceCRM.Server.Model.Dtos.DataTable;
using AutoMapper;
using TVServiceCRM.Server.Model.Dtos.Identity;
using TVServiceCRM.Server.Model.Dtos;

namespace TVServiceCRM.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClaimsController : ControllerBase
    {
        private readonly IDataService _dataService;
        private readonly ILogger<TicketsController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly TokenSettings _tokenSettings;
        private readonly ClaimsIdentity _claimsIdentity;

        public ClaimsController(
            IDataService dataService,
            ILogger<TicketsController> logger,
            IMapper mapper,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager,
            ClaimsIdentity claimsIdentity,
            TokenSettings tokenSettings)
        {
            _dataService = dataService;
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _claimsIdentity = claimsIdentity;
            _tokenSettings = tokenSettings;
        }


        // POST: api/Claims/GetDataTable
        [HttpPost("GetDataTable")]
        public async Task<ApiResponse<DataTableDto<IdentityClaimDto>>> GetDataTable([FromBody] DataTableDto<IdentityClaimDto> dataTable)
        {
            List<IdentityClaimDto> identityClaimsDto = new List<IdentityClaimDto>();
            List<string> controllerNames = _claimsIdentity.Claims
                .Select(x => x.Value.Split('_')[0])
                .Distinct()
                .ToList();


            string? roleName = dataTable.Filters?.RoleName?.Value;
            if (roleName != null && roleName.Count() > 0)
            {
                IdentityRole? identityRole = await _roleManager.FindByNameAsync(roleName);
                if (identityRole != null)
                {
                    List<Claim> roleClaims = (await _roleManager.GetClaimsAsync(identityRole)).ToList();

                    foreach (var controller in controllerNames)
                        identityClaimsDto.Add(new IdentityClaimDto()
                        {
                            Controller = controller,
                            View = roleClaims.Any(x => x.Value == controller + "_View"),
                            Add = roleClaims.Any(x => x.Value == controller + "_Add"),
                            Edit = roleClaims.Any(x => x.Value == controller + "_Edit"),
                            Delete = roleClaims.Any(x => x.Value == controller + "_Delete"),
                        });
                }
            }
            else
                foreach (var controller in controllerNames)
                    identityClaimsDto.Add(new IdentityClaimDto()
                    {
                        Controller = controller,
                        View = false,
                        Add = false,
                        Edit = false,
                        Delete = false,
                    });


            //var claimstest = _claimsIdentity.Claims
            //    .Select(x => x.Value)
            //    .Select(x => new { controller = x.Split('_')[0], claim = x.Split('_')[1] })
            //    .ToList();



            dataTable.Data = identityClaimsDto;
            return new ApiResponse<DataTableDto<IdentityClaimDto>>().SetSuccessResponse(dataTable);
        }
    }
}
