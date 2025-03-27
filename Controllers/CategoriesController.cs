using ECommerceAdminUI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace ECommerceAdminUI.Controllers;

public class CategoriesController(IHttpClientFactory httpClientFactory) : MyBaseController(httpClientFactory)
{
    [HttpGet]
    public async Task<IActionResult> Add()
    {
        HttpClient client = CreateClientAndFillAuthenticationHeaderValueWithAccessToken();
        HttpResponseMessage httpResponseMessage = await client.GetAsync($"{ApiUri}{ControllerName}/OData?$select=id,fullCategoryName");

        if (await HandleHttpResponseMessage(httpResponseMessage))
            return RedirectToAction(ActionName, ControllerName);

        string responseJsonData = await httpResponseMessage.Content.ReadAsStringAsync();

        List<CategorySelectBoxModel>? paginationCategories =
            JsonConvert.DeserializeObject<List<CategorySelectBoxModel>>(responseJsonData);
        ViewBag.Categories = paginationCategories;

        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Add(CategoryRequestAddModel categoryRequestAddModel)
    {
        HttpClient client = CreateClientAndFillAuthenticationHeaderValueWithAccessToken();
        HttpResponseMessage httpResponseMessage = await client.PostAsync($"{ApiUri}{ControllerName}/",
            new StringContent(JsonConvert.SerializeObject(categoryRequestAddModel), Encoding.UTF8, "application/json"));

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

        PaginationResponseViewModel<CategoryResponseViewModel>? paginationCategories =
            JsonConvert.DeserializeObject<PaginationResponseViewModel<CategoryResponseViewModel>>(responseJsonData);

        return View(paginationCategories);
    }
    [HttpGet]
    public async Task<IActionResult> ChangeStatus(int id, string callPage, int pageIndex, int topCategoryId)
    {
        HttpClient client = CreateClientAndFillAuthenticationHeaderValueWithAccessToken();
        HttpResponseMessage httpResponseMessage = await client.GetAsync($"{ApiUri}{ControllerName}/{id}");

        if (await HandleHttpResponseMessage(httpResponseMessage))
            return RedirectToAction(callPage, ControllerName, callPage == "GetList" ? 
                new { PageIndex = pageIndex } : new { TopCategoryId = topCategoryId });

        string responseJsonData = await httpResponseMessage.Content.ReadAsStringAsync();

        CategoryResponseViewModel? category =
            JsonConvert.DeserializeObject<CategoryResponseViewModel>(responseJsonData);

        if (category!.IsProductCategorization)
            category.IsProductCategorization = false;
        else category.IsProductCategorization = true;

        HttpResponseMessage updateMessage = await client.PutAsync($"{ApiUri}{ControllerName}/",
            new StringContent(JsonConvert.SerializeObject(category), Encoding.UTF8, "application/json"));

        if (await HandleHttpResponseMessage(httpResponseMessage))
            return RedirectToAction(callPage, ControllerName, callPage == "GetList" ?
                new { PageIndex = pageIndex } : new { TopCategoryId = topCategoryId });

        return NoContent();
    }
    [HttpGet]
    public async Task<IActionResult> Delete(int id, string callPage, int pageIndex, int topCategoryId)
    {
        HttpClient client = CreateClientAndFillAuthenticationHeaderValueWithAccessToken();
        HttpResponseMessage httpResponseMessage = await client.DeleteAsync($"{ApiUri}{ControllerName}/{id}");

        if (await HandleHttpResponseMessage(httpResponseMessage))
            return RedirectToAction(callPage, ControllerName, callPage == "GetList" ?
                new { PageIndex = pageIndex } : new { TopCategoryId = topCategoryId });

        return RedirectToAction(callPage, ControllerName, callPage == "GetList" ?
                new { PageIndex = pageIndex } : new { TopCategoryId = topCategoryId });
    }
    [HttpGet]
    public async Task<IActionResult> GetListNested(int topCategoryId = 1)
    {
        HttpClient client = CreateClientAndFillAuthenticationHeaderValueWithAccessToken();
        HttpResponseMessage httpResponseMessage =
            await client.GetAsync($"{ApiUri}{ControllerName}/OData?filter=TopCategoryId eq {topCategoryId}");


        if (await HandleHttpResponseMessage(httpResponseMessage))
            return RedirectToAction(ActionName, ControllerName, new { TopCategoryId = topCategoryId });

        string responseJsonData = await httpResponseMessage.Content.ReadAsStringAsync();

        List<CategoryResponseViewModel>? categories =
            JsonConvert.DeserializeObject<List<CategoryResponseViewModel>>(responseJsonData);

        return View(categories);
    }
}
