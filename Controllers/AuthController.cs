using ECommerceAdminUI.Controllers.Extensions;
using ECommerceAdminUI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace ECommerceAdminUI.Controllers;
public class AuthController(IHttpClientFactory httpClientFactory) : MyBaseController(httpClientFactory)
{
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Login(AuthLoginRequestModel authLoginRequestModel)
    {
        HttpClient httpClient = _httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en"));

        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync($"{ApiUri}{ControllerName}/{ActionName}/",
            new StringContent(JsonConvert.SerializeObject(authLoginRequestModel), Encoding.UTF8,"application/json"));

        if(await HandleHttpResponseMessage(httpResponseMessage))
            return RedirectToAction(ActionName, ControllerName);

        string responseJsonData = await httpResponseMessage.Content.ReadAsStringAsync();
        AuthLoginResponseModel? authLoginResponseModel = JsonConvert.DeserializeObject<AuthLoginResponseModel>(responseJsonData);
        HttpContext.Session.SetString("accessToken", JsonConvert.SerializeObject(authLoginResponseModel.accessToken));

        RefreshToken refreshToken = httpResponseMessage.GetRefreshTokenFromCookies();
        HttpContext.Session.SetString("refreshToken", JsonConvert.SerializeObject(refreshToken));

        return RedirectToAction("Index", "Home");
    }
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Register(AuthRegisterRequestModel authRegisterRequestModel)
    {
        HttpClient httpClient = _httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en"));

        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync($"{ApiUri}{ControllerName}/{ActionName}/",
            new StringContent(JsonConvert.SerializeObject(authRegisterRequestModel), Encoding.UTF8, "application/json"));

        if (await HandleHttpResponseMessage(httpResponseMessage))
            return RedirectToAction(ActionName, ControllerName);

        string responseJsonData = await httpResponseMessage.Content.ReadAsStringAsync();
        AccessToken? accessToken = JsonConvert.DeserializeObject<AccessToken>(responseJsonData);
        HttpContext.Session.SetString("accessToken", JsonConvert.SerializeObject(accessToken));

        RefreshToken refreshToken = httpResponseMessage.GetRefreshTokenFromCookies();
        HttpContext.Session.SetString("refreshToken", JsonConvert.SerializeObject(refreshToken));

        return RedirectToAction("Index", "Home");
    }
    [HttpGet]
    public async Task<IActionResult> RefreshToken()
    {
        if(AccessToken==null)
            return RedirectToAction("Login", ControllerName);

        HttpClient httpClient = CreateClientAndFillBothAuthenticationHeaderValueWithAccessTokenAndCookieWithRefreshToken();
        HttpResponseMessage httpResponseMessage = await httpClient.GetAsync($"{ApiUri}{ControllerName}/{ActionName}");

        string jsonResponseData = await httpResponseMessage.Content.ReadAsStringAsync();
        AccessToken? newAccessToken = JsonConvert.DeserializeObject<AccessToken>(jsonResponseData);
        HttpContext.Session.SetString("accessToken", JsonConvert.SerializeObject(newAccessToken));

        RefreshToken newRefreshToken = httpResponseMessage.GetRefreshTokenFromCookies();
        HttpContext.Session.SetString("refreshToken", JsonConvert.SerializeObject(newRefreshToken));

        return NoContent();
    }
}