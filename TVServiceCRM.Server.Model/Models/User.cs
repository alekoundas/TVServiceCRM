using Microsoft.AspNetCore.Identity;

namespace TVServiceCRM.Server.Model.Models
{
    public class User : IdentityUser
    {
        public string RoleId { get; set; } = "";
    }
}
