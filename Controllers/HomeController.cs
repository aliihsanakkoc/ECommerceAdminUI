using Microsoft.AspNetCore.Mvc;

namespace ECommerceAdminUI.Controllers;

public class HomeController(IHttpClientFactory httpClientFactory) : MyBaseController(httpClientFactory)
{
    public IActionResult Index()
    {
        return View();
    }
}
