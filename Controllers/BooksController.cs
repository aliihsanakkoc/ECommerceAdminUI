using ECommerceAdminUI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ECommerceAdminUI.Controllers;

public class BooksController(IHttpClientFactory httpClientFactory) : MyBaseController(httpClientFactory)
{
    [HttpGet]
    public IActionResult Add(int productId, int pageIndex)
    {
        ViewBag.ProductId = productId;
        ViewBag.PageIndex = pageIndex;
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Add(BookRequestAddModel book, int pageIndex)
    {
        HttpClient client = CreateClientAndFillAuthenticationHeaderValueWithAccessToken();  
        HttpResponseMessage response = await client.PostAsJsonAsync($"{ApiUri}{ControllerName}/", book);

        if(await HandleHttpResponseMessage(response))
            return RedirectToAction("GetList", "Products", new { PageIndex = pageIndex });

        return RedirectToAction("GetList", "Products", new { PageIndex = pageIndex });
    }
    [HttpGet]
    public async Task<IActionResult> Update(int productId, int pageIndex)
    {
        
        HttpClient client = CreateClientAndFillAuthenticationHeaderValueWithAccessToken();
        HttpResponseMessage response = await client.GetAsync($"{ApiUri}{ControllerName}/{productId}");

        if(await HandleHttpResponseMessage(response))
            return RedirectToAction("GetList", "Products", new { PageIndex = pageIndex });

        BookResponseViewModel book = JsonConvert.DeserializeObject<BookResponseViewModel>(await response.Content.ReadAsStringAsync())!; 

        ViewBag.PageIndex = pageIndex;
        return View(book);
    }
    [HttpPost]
    public async Task<IActionResult> Update(BookResponseViewModel book, int pageIndex)
    {
        HttpClient client = CreateClientAndFillAuthenticationHeaderValueWithAccessToken();
        HttpResponseMessage response = await client.PutAsJsonAsync($"{ApiUri}{ControllerName}/", book);

        if (await HandleHttpResponseMessage(response))
            return RedirectToAction("GetList", "Products", new { PageIndex = pageIndex });

        return RedirectToAction("GetList", "Products", new { PageIndex = pageIndex });
    }
}
