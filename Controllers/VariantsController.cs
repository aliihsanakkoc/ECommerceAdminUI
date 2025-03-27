using ECommerceAdminUI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ECommerceAdminUI.Controllers;

public class VariantsController(IHttpClientFactory httpClientFactory) : MyBaseController(httpClientFactory)
{
    [HttpGet]
    public async Task<IActionResult> Add()
    {
        HttpClient client = CreateClientAndFillAuthenticationHeaderValueWithAccessToken();
        HttpResponseMessage httpResponseMessage = await client.GetAsync($"{ApiUri}{ControllerName}/OData?$filter=TopVariantId eq 1");

        if (await HandleHttpResponseMessage(httpResponseMessage))
            return RedirectToAction(ActionName, ControllerName);

        string responseJsonData = await httpResponseMessage.Content.ReadAsStringAsync();

        List<VariantResponseViewModel>? variants =
            JsonConvert.DeserializeObject<List<VariantResponseViewModel>>(responseJsonData);
        ViewBag.Variants = variants;

        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Add(VariantRequestAddModel variant)
    {
        HttpClient client = CreateClientAndFillAuthenticationHeaderValueWithAccessToken();
        HttpResponseMessage httpResponseMessage = await client.PostAsJsonAsync($"{ApiUri}{ControllerName}/", variant);

        if (await HandleHttpResponseMessage(httpResponseMessage))
            return RedirectToAction(ActionName, ControllerName);

        return RedirectToAction(ActionName, ControllerName);
    }
    [HttpGet]
    public async Task<IActionResult> GetList(int pageIndex = 0, int pageSize = 10)
    {
        HttpClient client = CreateClientAndFillAuthenticationHeaderValueWithAccessToken();
        HttpResponseMessage httpResponseMessage =
            await client.GetAsync($"{ApiUri}{ControllerName}?PageIndex={pageIndex}&PageSize={pageSize}");

        if (await HandleHttpResponseMessage(httpResponseMessage))
            return RedirectToAction(ActionName, ControllerName, new { PageIndex = pageIndex });

        string responseJsonData = await httpResponseMessage.Content.ReadAsStringAsync();

        PaginationResponseViewModel<VariantResponseViewModel>? paginationVariants =
            JsonConvert.DeserializeObject<PaginationResponseViewModel<VariantResponseViewModel>>(responseJsonData);

        return View(paginationVariants);
    }
    [HttpGet]
    public async Task<IActionResult> Delete(int id, string callPage, int pageIndex, int topVariantId)
    {
        HttpClient client = CreateClientAndFillAuthenticationHeaderValueWithAccessToken();
        HttpResponseMessage httpResponseMessage = await client.DeleteAsync($"{ApiUri}{ControllerName}/{id}");

        if (await HandleHttpResponseMessage(httpResponseMessage))
            return RedirectToAction(callPage, ControllerName, callPage == "GetList" ?
                new { PageIndex = pageIndex } : new { TopVariantId = topVariantId });

        return RedirectToAction(callPage, ControllerName, callPage == "GetList" ?
                new { PageIndex = pageIndex } : new { TopVariantId = topVariantId });
    }
    [HttpGet]
    public async Task<IActionResult> GetListNested(int topVariantId = 1)
    {
        HttpClient client = CreateClientAndFillAuthenticationHeaderValueWithAccessToken();
        HttpResponseMessage httpResponseMessage =
            await client.GetAsync($"{ApiUri}{ControllerName}/OData?filter=TopVariantId eq {topVariantId}");


        if (await HandleHttpResponseMessage(httpResponseMessage))
            return RedirectToAction(ActionName, ControllerName, new { TopVariantId = topVariantId });

        string responseJsonData = await httpResponseMessage.Content.ReadAsStringAsync();

        List<VariantResponseViewModel>? variants =
            JsonConvert.DeserializeObject<List<VariantResponseViewModel>>(responseJsonData);

        return View(variants);
    }
}
