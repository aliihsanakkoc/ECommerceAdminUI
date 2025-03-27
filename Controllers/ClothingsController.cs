using ECommerceAdminUI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ECommerceAdminUI.Controllers;

public class ClothingsController(IHttpClientFactory httpClientFactory) : MyBaseController(httpClientFactory)
{
    [HttpGet]
    public IActionResult Add(int productId, int pageIndex)
    {
        ViewBag.ProductId = productId;
        ViewBag.PageIndex = pageIndex;
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Add(ClothingRequestAddModel clothing, int pageIndex)
    {
        HttpClient client = CreateClientAndFillAuthenticationHeaderValueWithAccessToken();
        HttpResponseMessage response = await client.PostAsJsonAsync($"{ApiUri}{ControllerName}/", clothing);

        if (await HandleHttpResponseMessage(response))
            return RedirectToAction("GetList", "Products", new { PageIndex = pageIndex });

        return RedirectToAction("GetList", "Products", new { PageIndex = pageIndex });
    }
    [HttpGet]
    public async Task<IActionResult> Update(int productId, int pageIndex)
    {

        HttpClient client = CreateClientAndFillAuthenticationHeaderValueWithAccessToken();
        HttpResponseMessage response = await client.GetAsync($"{ApiUri}{ControllerName}/{productId}");

        if (await HandleHttpResponseMessage(response))
            return RedirectToAction("GetList", "Products", new { PageIndex = pageIndex });

        ClothingResponseViewModel book = JsonConvert.DeserializeObject<ClothingResponseViewModel>(await response.Content.ReadAsStringAsync())!;

        ViewBag.PageIndex = pageIndex;
        return View(book);
    }
    [HttpPost]
    public async Task<IActionResult> Update(ClothingResponseViewModel clothing, int pageIndex)
    {
        HttpClient client = CreateClientAndFillAuthenticationHeaderValueWithAccessToken();
        HttpResponseMessage response = await client.PutAsJsonAsync($"{ApiUri}{ControllerName}/", clothing);

        if (await HandleHttpResponseMessage(response))
            return RedirectToAction("GetList", "Products", new { PageIndex = pageIndex });

        return RedirectToAction("GetList", "Products", new { PageIndex = pageIndex });
    }
}
