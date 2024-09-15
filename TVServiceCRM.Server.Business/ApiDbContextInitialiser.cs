﻿using Microsoft.AspNetCore.Identity;
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
        private readonly ClaimsIdentity _claimsIdentity;



        public ApiDbContextInitialiser(
            IDataService dataService,
            ILogger<ApiDbContextInitialiser> logger,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ClaimsIdentity claimsIdentity)
        {
            _dataService = dataService;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _claimsIdentity = claimsIdentity;
        }

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
                TrySeedClaimsAsync();
                await TrySeedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        private void TrySeedClaimsAsync()
        {
            _claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Customers_View"));
            _claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Customers_Add"));
            _claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Customers_Edit"));
            _claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Customers_Delete"));

            _claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Tickets_View"));
            _claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Tickets_Add"));
            _claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Tickets_Edit"));
            _claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Tickets_Delete"));

            _claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "ContactInformations_View"));
            _claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "ContactInformations_Add"));
            _claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "ContactInformations_Edit"));
            _claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "ContactInformations_Delete"));

            _claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Roles_View"));
            _claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Roles_Add"));
            _claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Roles_Edit"));
            _claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Roles_Delete"));

            _claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Users_View"));
            _claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Users_Add"));
            _claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Users_Edit"));
            _claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Users_Delete"));

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

                    _claimsIdentity.Claims
                        .Select(x => x.Value)
                        .ToList()
                        .ForEach(async x =>
                            await _roleManager.AddClaimAsync(administratorRole, _claimsIdentity.Claims.First(y => y.Value == x))
                         );

                    //await _roleManager.AddClaimAsync(administratorRole, _claimsIdentity.Claims.First(x => x.Value == "Customer_Add"));
                    //await _roleManager.AddClaimAsync(administratorRole, _claimsIdentity.Claims.First(x => x.Value == "Customer_Edit"));
                    //await _roleManager.AddClaimAsync(administratorRole, _claimsIdentity.Claims.First(x => x.Value == "Customer_Delete"));

                    //await _roleManager.AddClaimAsync(administratorRole, _claimsIdentity.Claims.First(x => x.Value == "Ticket_View"));
                    //await _roleManager.AddClaimAsync(administratorRole, _claimsIdentity.Claims.First(x => x.Value == "Ticket_Add"));
                    //await _roleManager.AddClaimAsync(administratorRole, _claimsIdentity.Claims.First(x => x.Value == "Ticket_Edit"));
                    //await _roleManager.AddClaimAsync(administratorRole, _claimsIdentity.Claims.First(x => x.Value == "Ticket_Delete"));

                    //await _roleManager.AddClaimAsync(administratorRole, _claimsIdentity.Claims.First(x => x.Value == "ContactInformation_View"));
                    //await _roleManager.AddClaimAsync(administratorRole, _claimsIdentity.Claims.First(x => x.Value == "ContactInformation_Add"));
                    //await _roleManager.AddClaimAsync(administratorRole, _claimsIdentity.Claims.First(x => x.Value == "ContactInformation_Edit"));
                    //await _roleManager.AddClaimAsync(administratorRole, _claimsIdentity.Claims.First(x => x.Value == "ContactInformation_Delete"));

                    //await _roleManager.AddClaimAsync(administratorRole, _claimsIdentity.Claims.First(x => x.Value == "Role_View"));
                    //await _roleManager.AddClaimAsync(administratorRole, _claimsIdentity.Claims.First(x => x.Value == "Role_Add"));
                    //await _roleManager.AddClaimAsync(administratorRole, _claimsIdentity.Claims.First(x => x.Value == "Role_Edit"));
                    //await _roleManager.AddClaimAsync(administratorRole, _claimsIdentity.Claims.First(x => x.Value == "Role_Delete"));
                }
            }

            // Default users
            var administrator = new ApplicationUser { UserName = "Admin", Email = "Admin@Admin.Admin" };

            if (_userManager.Users.All(u => u.UserName != administrator.UserName))
            {
                await _userManager.CreateAsync(administrator, "P@ssw0rd");
                if (!string.IsNullOrWhiteSpace(administratorRole.Name))
                {
                    await _userManager.AddToRolesAsync(administrator, new[] { administratorRole.Name });
                }
            }
        }
    }
}