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
                _claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Customer_View"));
                _claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Customer_Add"));
                _claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Customer_Edit"));
                _claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Customer_Delete"));

                _claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Ticket_View"));
                _claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Ticket_Add"));
                _claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Ticket_Edit"));
                _claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Ticket_Delete"));

                _claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "ContactInformation_View"));
                _claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "ContactInformation_Add"));
                _claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "ContactInformation_Edit"));
                _claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "ContactInformation_Delete"));

                _claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Role_View"));
                _claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Role_Add"));
                _claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Role_Edit"));
                _claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Role_Delete"));

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
                    await _roleManager.AddClaimAsync(administratorRole, _claimsIdentity.Claims.First(x=>x.Value== "Customer_View"));
                    await _roleManager.AddClaimAsync(administratorRole, _claimsIdentity.Claims.First(x=>x.Value== "Customer_Add"));
                    await _roleManager.AddClaimAsync(administratorRole, _claimsIdentity.Claims.First(x=>x.Value== "Customer_Edit"));
                    await _roleManager.AddClaimAsync(administratorRole, _claimsIdentity.Claims.First(x=>x.Value== "Customer_Delete"));

                    await _roleManager.AddClaimAsync(administratorRole, _claimsIdentity.Claims.First(x => x.Value == "Ticket_View"));
                    await _roleManager.AddClaimAsync(administratorRole, _claimsIdentity.Claims.First(x => x.Value == "Ticket_Add"));
                    await _roleManager.AddClaimAsync(administratorRole, _claimsIdentity.Claims.First(x => x.Value == "Ticket_Edit"));
                    await _roleManager.AddClaimAsync(administratorRole, _claimsIdentity.Claims.First(x => x.Value == "Ticket_Delete"));

                    await _roleManager.AddClaimAsync(administratorRole, _claimsIdentity.Claims.First(x => x.Value == "ContactInformation_View"));
                    await _roleManager.AddClaimAsync(administratorRole, _claimsIdentity.Claims.First(x => x.Value == "ContactInformation_Add"));
                    await _roleManager.AddClaimAsync(administratorRole, _claimsIdentity.Claims.First(x => x.Value == "ContactInformation_Edit"));
                    await _roleManager.AddClaimAsync(administratorRole, _claimsIdentity.Claims.First(x => x.Value == "ContactInformation_Delete"));

                    await _roleManager.AddClaimAsync(administratorRole, _claimsIdentity.Claims.First(x => x.Value == "Role_View"));
                    await _roleManager.AddClaimAsync(administratorRole, _claimsIdentity.Claims.First(x => x.Value == "Role_Add"));
                    await _roleManager.AddClaimAsync(administratorRole, _claimsIdentity.Claims.First(x => x.Value == "Role_Edit"));
                    await _roleManager.AddClaimAsync(administratorRole, _claimsIdentity.Claims.First(x => x.Value == "Role_Delete"));
                }
            }

            // Default users
            var administrator = new ApplicationUser { UserName = "UnifiedAppAdmin1!", Email = "UnifiedAppAdmin1!" };

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