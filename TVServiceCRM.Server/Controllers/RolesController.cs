using Microsoft.AspNetCore.Mvc;
using TVServiceCRM.Server.Model.Models;
using TVServiceCRM.Server.Business.IServices;
using Microsoft.AspNetCore.Identity;
using TVServiceCRM.Server.Model.Dtos;
using System.Security.Claims;
using TVServiceCRM.Server.Model.System;
using Microsoft.EntityFrameworkCore;
using TVServiceCRM.Server.Model.Dtos.DataTable;
using AutoMapper;
using TVServiceCRM.Server.Model.Dtos.Identity;

namespace TVServiceCRM.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IDataService _dataService;
        private readonly ILogger<TicketsController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly TokenSettings _tokenSettings;

        public RolesController(
            IDataService dataService,
            ILogger<TicketsController> logger,
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            TokenSettings tokenSettings)
        {
            _dataService = dataService;
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _tokenSettings = tokenSettings;
        }

        // GET: api/IdentityRoles
        [HttpGet]
        public async Task<IEnumerable<IdentityRole>> GetAll()
        {
            List<IdentityRole> result = await _roleManager.Roles.ToListAsync();
            return result;
        }

        // POST: api/IdentityRoles/GetDataTable
        [HttpPost("GetDataTable")]
        public async Task<DataTableDto<IdentityRoleDto>> GetDataTable([FromBody] DataTableDto<IdentityRoleDto> dataTable)
        {
            //Func<IQueryable<IdentityRole>, IOrderedQueryable<IdentityRole>>? orderByQuery = null;
            //List<Func<IOrderedQueryable<IdentityRole>, IOrderedQueryable<IdentityRole>>>? thenOrderByQuery = new List<Func<IOrderedQueryable<IdentityRole>, IOrderedQueryable<IdentityRole>>>();
            //Expression<Func<IdentityRole, bool>>? filterQuery = new Expression<Func<IdentityRole, bool>>();


            //// Handle Sorting of DataTable.
            //if (dataTable.MultiSortMeta?.Count() > 0)
            //{
            //    // Create the first OrderBy().
            //    DataTableSortDto? dataTableSort = dataTable.MultiSortMeta.First();
            //    if (dataTableSort.Order > 0)
            //        orderByQuery = x => x.OrderByColumn(dataTableSort.Field);
            //    else if (dataTableSort.Order < 0)
            //        orderByQuery = x => x.OrderByColumnDescending(dataTableSort.Field);

            //    // Create the rest OrderBy methods as ThenBy() if any.
            //    foreach (var sortInfo in dataTable.MultiSortMeta.Skip(1))
            //    {
            //        if (dataTableSort.Order > 0)
            //            thenOrderByQuery.Add(x => x.ThenByColumn(sortInfo.Field));
            //        else if (dataTableSort.Order < 0)
            //            thenOrderByQuery.Add(x => x.ThenByColumnDescending(sortInfo.Field));
            //    }
            //}


            // Handle Filtering of DataTable.
            //if (dataTable.Filters?.FirstName?.Value != null && dataTable.Filters?.FirstName.Value.Length > 0)
            //    filterQuery.Add(x => x.FirstName.Contains(dataTable.Filters.FirstName.Value));

            //if (dataTable.Filters?.LastName?.Value != null && dataTable.Filters?.LastName.Value.Length > 0)
            //    filterQuery.Add(x => x.LastName.Contains(dataTable.Filters.LastName.Value));



            // Retrieve Data.
            List<IdentityRole> result = await _dataService.Query.Roles.ToListAsync();
                
                
                
            //    (
            //    filterQuery,
            //    orderByQuery,
            //    thenOrderByQuery,
            //    null,
            //    dataTable.PageCount.Value,
            //    dataTable.Page.Value
            //);
            List<IdentityRoleDto> IdentityRoleDto = _mapper.Map<List<IdentityRoleDto>>(result);

            //IdentityRoleDto.SelectMany(x => x.ContactInformations).ToList().ForEach(x => x.IdentityRole = null);
            //IdentityRoleDto.SelectMany(x => x.Tickets).ToList().ForEach(x => x.IdentityRole = null);

            //int rows = await _dataService.IdentityRoles.co(filterQuery);

            dataTable.Data = IdentityRoleDto;
            dataTable.PageCount = 0;
            return dataTable;

        }

        // Create a new role
        [HttpPost]
        public async Task<ApiResponse<IdentityRoleDto>> CreateRole([FromBody] IdentityRoleDto identityRoleDto)
        {
            IdentityRole identityRole = _mapper.Map<IdentityRole>(identityRoleDto);
            if (string.IsNullOrWhiteSpace(identityRole.Name))
                return new ApiResponse<IdentityRoleDto>().SetErrorResponse("errors", "Role name is required");


            var roleExists = await _roleManager.RoleExistsAsync(identityRole.Name);
            if (roleExists)
                return new ApiResponse<IdentityRoleDto>().SetErrorResponse("errors", "Role already exists");

            var result = await _roleManager.CreateAsync(identityRole);
            if (result.Succeeded)
                return new ApiResponse<IdentityRoleDto>().SetSuccessResponse(identityRoleDto);

            return new ApiResponse<IdentityRoleDto>().SetErrorResponse("errors", result.Errors.ToString());
        }

        // Add a claim to a role
        [HttpPost("{roleName}/claims")]
        public async Task<ApiResponse<List<IdentityClaimDto>>> AddClaimToRole(string roleName, [FromBody] List<IdentityClaimDto> identityClaimsDto)
        {
            foreach (var claimDto in identityClaimsDto)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role == null)
                    return new ApiResponse<List<IdentityClaimDto>>().SetErrorResponse("errors", "Role not found.");

                var claim = new Claim(claimDto.ClaimType, claimDto.ClaimValue);
                var result = await _roleManager.AddClaimAsync(role, claim);

                if (!result.Succeeded)
                    return new ApiResponse<List<IdentityClaimDto>>().SetErrorResponse("errors", "Role not added.");
            }
            return new ApiResponse<List<IdentityClaimDto>>().SetSuccessResponse(identityClaimsDto);

        }

        // Get all claims for a role
        [HttpGet("{roleName}/claims")]
        public async Task<IActionResult> GetRoleClaims(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
                return NotFound("Role not found");

            var claims = await _roleManager.GetClaimsAsync(role);
            return Ok(claims);
        }

        // Remove a claim from a role
        //[HttpDelete("{roleName}/claims")]
        //public async Task<IActionResult> RemoveClaimFromRole(string roleName, [FromBody] ClaimModel model)
        //{
        //    var role = await _roleManager.FindByNameAsync(roleName);
        //    if (role == null)
        //        return NotFound("Role not found");

        //    var claim = new Claim(model.ClaimType, model.ClaimValue);
        //    var result = await _roleManager.RemoveClaimAsync(role, claim);

        //    if (result.Succeeded)
        //        return Ok($"Claim removed from role {roleName} successfully");

        //    return BadRequest(result.Errors);
        //}


        // Delete a role
        [HttpDelete("{roleName}")]
        public async Task<IActionResult> DeleteRole(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
                return NotFound("Role not found");

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
                return Ok($"Role {roleName} deleted successfully");

            return BadRequest(result.Errors);
        }



    }

}
