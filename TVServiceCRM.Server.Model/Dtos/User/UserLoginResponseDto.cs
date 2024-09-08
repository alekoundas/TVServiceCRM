
namespace TVServiceCRM.Server.Model.Dtos
{
    public class UserLoginResponseDto
    {
        public string AccessToken { get; set; } = "";
        public string RefreshToken { get; set; } = "";
    }
}
