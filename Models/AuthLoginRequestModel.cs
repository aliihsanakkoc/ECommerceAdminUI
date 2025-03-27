namespace ECommerceAdminUI.Models;

public class AuthLoginRequestModel
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string AuthenticatorCode { get; set; }
}
