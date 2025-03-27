using ECommerceAdminUI.Models;

namespace ECommerceAdminUI.Controllers.Extensions;

public static class HttpResponseMessageExtensions
{
    public static RefreshToken GetRefreshTokenFromCookies(this HttpResponseMessage message)
    {
        RefreshToken refreshToken = new();
        if (message.Headers.TryGetValues("Set-Cookie", out var cookies))
        {
            var refreshTokenCookie = cookies.FirstOrDefault(c => c.Contains("refreshToken"));
            var refreshTokenParts = refreshTokenCookie?.Split(';');
            refreshToken.Token = refreshTokenParts[0].Replace("refreshToken=", "");
            refreshToken.ExpirationDate = DateTime.Parse(refreshTokenParts[1].Replace("expires=", ""));
        }
        return refreshToken;
    }
}
