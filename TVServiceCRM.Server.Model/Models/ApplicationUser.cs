using Microsoft.AspNetCore.Identity;

namespace TVServiceCRM.Server.Model.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string RoleId { get; set; } = "";
    }
}
