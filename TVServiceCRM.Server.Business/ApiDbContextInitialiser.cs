using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using TVServiceCRM.Server.Business.IServices;
using TVServiceCRM.Server.Model.Models;

namespace TVServiceCRM.Server.Business
{
    public class ApiDbContextInitialiser
    {
        private readonly IDataService _dataService;
        private readonly ILogger<ApiDbContextInitialiser> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;



        public ApiDbContextInitialiser(
            IDataService dataService,
            ILogger<ApiDbContextInitialiser> logger,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _dataService = dataService;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        //public async Task InitialiseAsync()
        //{
        //    try
        //    {
        //        if (_dataService.Database.IsSqlServer())
        //        {
        //            await _context.Database.MigrateAsync();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred while initialising the database.");
        //        throw;
        //    }
        //}

        public async Task RunMigrationsAsync()
        {
            try
            {
                if (_dataService.Query.Database.GetPendingMigrations().Any())
                    await _dataService.Query.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while executing migrations on the database.");
                throw;
            }
        }


        public async Task SeedAsync()
        {
            try
            {
                await TrySeedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        private async Task TrySeedAsync()
        {
            // Default roles
            var administratorRole = new IdentityRole("Administrator");

            if (_roleManager.Roles.All(r => r.Name != administratorRole.Name))
            {
                var role = await _roleManager.CreateAsync(administratorRole);
                if (role != null)
                {
                    await _roleManager.AddClaimAsync(administratorRole, new Claim("RoleClaim", "HasRoleView"));
                    await _roleManager.AddClaimAsync(administratorRole, new Claim("RoleClaim", "HasRoleAdd"));
                    await _roleManager.AddClaimAsync(administratorRole, new Claim("RoleClaim", "HasRoleEdit"));
                    await _roleManager.AddClaimAsync(administratorRole, new Claim("RoleClaim", "HasRoleDelete"));
                }
            }

            // Default users
            var administrator = new ApplicationUser { UserName = "UnifiedAppAdmin", Email = "UnifiedAppAdmin" };

            if (_userManager.Users.All(u => u.UserName != administrator.UserName))
            {
                await _userManager.CreateAsync(administrator, "UnifiedAppAdmin1!");
                if (!string.IsNullOrWhiteSpace(administratorRole.Name))
                {
                    await _userManager.AddToRolesAsync(administrator, new[] { administratorRole.Name });
                }
            }
        }
    }
}