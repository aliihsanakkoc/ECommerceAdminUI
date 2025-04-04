using ECommerceAdminUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace ECommerceAdminUI.Controllers;

public class MyBaseController : Controller
{
    protected readonly IHttpClientFactory _httpClientFactory;

    public MyBaseController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    protected string ApiUri => "http://localhost:60805/api/";
    protected string? ControllerName => ControllerContext.RouteData.Values["controller"]?.ToString();
    protected string? ActionName => ControllerContext.RouteData.Values["action"]?.ToString();
    protected string PaginationByPass => $"?PageIndex=0&PageSize={int.MaxValue}";
    protected AccessToken? AccessToken => (!HttpContext.Session.Keys.Contains("accessToken")) ? null :
        JsonConvert.DeserializeObject<AccessToken>(HttpContext.Session.GetString("accessToken")!);
    protected RefreshToken? refreshToken => (!HttpContext.Session.Keys.Contains("refreshToken")) ? null :
        JsonConvert.DeserializeObject<RefreshToken>(HttpContext.Session.GetString("refreshToken")!);
    protected HttpClient CreateClientAndFillAuthenticationHeaderValueWithAccessToken()
    {
        HttpClient client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken!.Token);
        client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en"));  
        return client;
    }
    protected HttpClient CreateClientAndFillBothAuthenticationHeaderValueWithAccessTokenAndCookieWithRefreshToken()
    {
        HttpClient client = CreateClientAndFillAuthenticationHeaderValueWithAccessToken();
        client.DefaultRequestHeaders.Add("Cookie", $"refreshToken={refreshToken!.Token}");
        return client;
    }
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (ControllerName == "Auth")
            return;
        if (AccessToken == null || string.IsNullOrEmpty(AccessToken.Token))
        {
            context.Result = RedirectToAction("Login", "Auth");
            return;
        }
        if (AccessToken.ExpirationDate < DateTime.Now)
        {
            context.Result = RedirectToAction("RefreshToken", "Auth");
            return;
        }
        base.OnActionExecuting(context);
    }
    protected async Task<bool> HandleHttpResponseMessage(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            string message = "";
            ExceptionViewModel? exceptionViewModel = 
                JsonConvert.DeserializeObject<ExceptionViewModel>(await response.Content.ReadAsStringAsync());

            if (exceptionViewModel == null)
            {
                TempData["ExceptionMessage"] = "An error occurred while processing your request.";
                return true;
            }
                
            if (exceptionViewModel!.Errors != null)
                foreach (Error error in exceptionViewModel.Errors)
                    message += error.Errors.GetValue(0)!.ToString();
            TempData["ExceptionMessage"] = $"{exceptionViewModel.Detail} {message}";
            return true;
        }
        return false;
    }
}
