namespace ECommerceAdminUI.Models
{
    public class AuthLoginResponseModel
    {
        public AccessToken accessToken {  get; set; }
        public string RequiredAuthenticatorType { get; set; }
    }
}
