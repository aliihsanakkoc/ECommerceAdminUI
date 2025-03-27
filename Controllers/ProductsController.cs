using ECommerceAdminUI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace ECommerceAdminUI.Controllers;
public class ProductsController(IHttpClientFactory httpClientFactory) : MyBaseController(httpClientFactory)
{
    [HttpGet]
    public async Task<IActionResult> Add()
    {
        HttpClient client = CreateClientAndFillAuthenticationHeaderValueWithAccessToken();
        HttpResponseMessage allCategoriesMessage = await client.GetAsync($"{ApiUri}Categories/" +
            $"OData?$select=id,fullCategoryName&$filter=isProductCategorization eq true");

        if (await HandleHttpResponseMessage(allCategoriesMessage))
            return RedirectToAction(ActionName, ControllerName);

        List<CategorySelectBoxModel>? allCategories = JsonConvert.DeserializeObject<
            List<CategorySelectBoxModel>>(await allCategoriesMessage.Content.ReadAsStringAsync());

        ViewBag.AllCategories = allCategories;

        HttpResponseMessage productTypesMessage = await client.GetAsync($"{ApiUri}{ControllerName}/ProductTypes"); 

        if (await HandleHttpResponseMessage(productTypesMessage))
            return RedirectToAction(ActionName, ControllerName);

        List<string> productTypes = JsonConvert.DeserializeObject<List<string>>(
            await productTypesMessage.Content.ReadAsStringAsync())!; 

        ViewBag.ProductTypes = productTypes;

        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Add(ProductRequestAddModel product, int[] categoryIds)
    {
        HttpClient client = CreateClientAndFillAuthenticationHeaderValueWithAccessToken();
        HttpResponseMessage httpResponseMessage = await client.PostAsJsonAsync($"{ApiUri}{ControllerName}/",product);

        if (await HandleHttpResponseMessage(httpResponseMessage))
            return RedirectToAction(ActionName, ControllerName);

        ProductResponseViewModel addedProduct = JsonConvert.DeserializeObject<ProductResponseViewModel>(
            await httpResponseMessage.Content.ReadAsStringAsync())!; 

        AddRangeCategoryProductsModel addRangeCategoryProductsModel = new()
        {
            ProductId = addedProduct.Id,
            CategoryIds = categoryIds
        };

        HttpResponseMessage addRangeCategoryProductsMessage = await client.PostAsJsonAsync(
            $"{ApiUri}CategoryProducts/AddRange", addRangeCategoryProductsModel);

        if (await HandleHttpResponseMessage(addRangeCategoryProductsMessage))
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

        PaginationResponseViewModel<ProductResponseViewModel>? paginationCategories =
            JsonConvert.DeserializeObject<PaginationResponseViewModel<ProductResponseViewModel>>(responseJsonData);

        return View(paginationCategories);
    }
    [HttpGet]
    public async Task<IActionResult> ChangeStatus(int id, int pageIndex)
    {
        HttpClient client = CreateClientAndFillAuthenticationHeaderValueWithAccessToken();
        HttpResponseMessage httpResponseMessage = await client.GetAsync($"{ApiUri}{ControllerName}/{id}");

        if (await HandleHttpResponseMessage(httpResponseMessage))
            return RedirectToAction("GetList", ControllerName, new { PageIndex = pageIndex } );

        string responseJsonData = await httpResponseMessage.Content.ReadAsStringAsync();

        ProductResponseViewModel? product =
            JsonConvert.DeserializeObject<ProductResponseViewModel>(responseJsonData);

        if (product!.IsAddToCart)
            product.IsAddToCart = false;
        else product.IsAddToCart = true;

        HttpResponseMessage updateMessage = await client.PutAsJsonAsync($"{ApiUri}{ControllerName}/",product);

        if (await HandleHttpResponseMessage(updateMessage))
            return RedirectToAction("GetList", ControllerName, new { PageIndex = pageIndex });

        return NoContent();
    }
    [HttpGet]
    public async Task<IActionResult> Delete(int id, int pageIndex)
    {
        HttpClient client = CreateClientAndFillAuthenticationHeaderValueWithAccessToken();
        HttpResponseMessage httpResponseMessage = await client.DeleteAsync($"{ApiUri}{ControllerName}/{id}");

        if (await HandleHttpResponseMessage(httpResponseMessage))
            return RedirectToAction("GetList", ControllerName, new { PageIndex = pageIndex });

        return RedirectToAction("GetList", ControllerName, new { PageIndex = pageIndex });
    }
    [HttpGet]
    public async Task<IActionResult> Update(int id, int pageIndex)
    {
        ViewBag.PageIndex = pageIndex;

        HttpClient client = CreateClientAndFillAuthenticationHeaderValueWithAccessToken();
        HttpResponseMessage allCategoriesMessage = await client.GetAsync($"{ApiUri}Categories/" +
            $"OData?$select=id,fullCategoryName&$filter=isProductCategorization eq true");

        if (await HandleHttpResponseMessage(allCategoriesMessage))
            return RedirectToAction("GetList", ControllerName, new {PageIndex = pageIndex});

        List<CategorySelectBoxModel>? allCategories = JsonConvert.DeserializeObject<
            List<CategorySelectBoxModel>>(await allCategoriesMessage.Content.ReadAsStringAsync());

        ViewBag.AllCategories = allCategories;

        HttpResponseMessage existCategoryProductsMessage = await client.GetAsync(
            $"{ApiUri}CategoryProducts/OData?$filter=productId eq {id}"); 
        
        if (await HandleHttpResponseMessage(existCategoryProductsMessage))
            return RedirectToAction("GetList", ControllerName, new { PageIndex = pageIndex });

        List<CategoryProductViewModel> existCategoryProducts = JsonConvert.DeserializeObject<
            List<CategoryProductViewModel>>(await existCategoryProductsMessage.Content.ReadAsStringAsync())!;   

        ViewBag.ExistCategoryIds = existCategoryProducts.Select(x => x.CategoryId).ToArray();   

        HttpResponseMessage productMessage = await client.GetAsync($"{ApiUri}{ControllerName}/{id}");   

        if (await HandleHttpResponseMessage(productMessage))
            return RedirectToAction("GetList", ControllerName, new { PageIndex = pageIndex });

        ProductResponseViewModel product = JsonConvert.DeserializeObject<
            ProductResponseViewModel>(await productMessage.Content.ReadAsStringAsync())!;

        HttpResponseMessage isLockedProductTypeMessage = await client.GetAsync($"{ApiUri}{product.ProductType}s/{id}");


        ViewBag.IsLockedProductType = false;
        if (isLockedProductTypeMessage.StatusCode== HttpStatusCode.OK)
            ViewBag.IsLockedProductType = true;

        HttpResponseMessage productTypesMessage = await client.GetAsync($"{ApiUri}{ControllerName}/ProductTypes");

        if (await HandleHttpResponseMessage(productTypesMessage))
            return RedirectToAction("GetList", ControllerName, new { PageIndex = pageIndex });

        List<string> productTypes = JsonConvert.DeserializeObject<List<string>>(
            await productTypesMessage.Content.ReadAsStringAsync())!;

        ViewBag.ProductTypes = productTypes;

        return View(product);
    }
    [HttpPost]
    public async Task<IActionResult> Update(ProductResponseViewModel product, int[] categoryIds, int pageIndex)
    {
        HttpClient client = CreateClientAndFillAuthenticationHeaderValueWithAccessToken();
        HttpResponseMessage httpResponseMessage = await client.PutAsJsonAsync($"{ApiUri}{ControllerName}/", product);

        if (await HandleHttpResponseMessage(httpResponseMessage))
            return RedirectToAction("GetList", ControllerName, new { PageIndex = pageIndex });

        HttpResponseMessage existCategoryProductsMessage = await client.GetAsync(
            $"{ApiUri}CategoryProducts/OData?$filter=productId eq {product.Id}");

        if (await HandleHttpResponseMessage(existCategoryProductsMessage))
            return RedirectToAction("GetList", ControllerName, new { PageIndex = pageIndex });

        List<CategoryProductViewModel> existCategoryProducts = JsonConvert.DeserializeObject<
            List<CategoryProductViewModel>>(await existCategoryProductsMessage.Content.ReadAsStringAsync())!;

        int[] existCategoryIds = existCategoryProducts.Select(x => x.CategoryId).ToArray();

        int[] addedCategoryIds = categoryIds.Except(existCategoryIds).ToArray();

        if (addedCategoryIds.Length > 0)
        {
            AddRangeCategoryProductsModel addRangeCategoryProductsModel = new()
            {
                ProductId = product.Id,
                CategoryIds = addedCategoryIds
            };

            HttpResponseMessage addRangeCategoryProductsMessage = await client.PostAsJsonAsync(
                $"{ApiUri}CategoryProducts/AddRange", addRangeCategoryProductsModel);

            if (await HandleHttpResponseMessage(addRangeCategoryProductsMessage))
                return RedirectToAction("GetList", ControllerName, new { PageIndex = pageIndex });
        }

        int[] deletedCategoryIds = existCategoryIds.Except(categoryIds).ToArray();

        if (deletedCategoryIds.Length > 0)
        {
            int[] deletedCategoryProductIds = existCategoryProducts.Where(x => deletedCategoryIds
                .Contains(x.CategoryId)).Select(x => x.Id).ToArray();

            DeleteRangeCategoryProductModel deleteRangeCategoryProductModel = new() 
                { Ids = deletedCategoryProductIds };

            HttpResponseMessage deleteRangeCategoryProductsMessage = await client.PostAsJsonAsync(
                $"{ApiUri}CategoryProducts/DeleteRange", deleteRangeCategoryProductModel);

            if (await HandleHttpResponseMessage(deleteRangeCategoryProductsMessage))
                return RedirectToAction("GetList", ControllerName, new { PageIndex = pageIndex });
        }

        return RedirectToAction("GetList", ControllerName, new { PageIndex = pageIndex });
    }
    public async Task<IActionResult> ProductTypeOperations(int id, int pageIndex, string productType)
    {
        HttpClient httpClient = CreateClientAndFillAuthenticationHeaderValueWithAccessToken();
        HttpResponseMessage httpResponseMessage = await httpClient.GetAsync($"{ApiUri}{productType}s/{id}");

        if (httpResponseMessage.IsSuccessStatusCode)
            return RedirectToAction("Update", $"{productType}s", new { ProductId = id, PageIndex = pageIndex });
        return RedirectToAction("Add", $"{productType}s", new { ProductId = id, PageIndex = pageIndex });

    }
    [HttpGet]
    public async Task<IActionResult> VariantOperations(int id, int pageIndex, string productName)
    {
        ViewBag.PageIndex = pageIndex;
        ViewBag.ProductName = productName;
        ViewBag.ProductId = id;

        HttpClient client = CreateClientAndFillAuthenticationHeaderValueWithAccessToken();
        HttpResponseMessage allVariantsMessage = await client.GetAsync($"{ApiUri}Variants/" +
            $"OData?$filter=topVariantId ne 1&$orderby=topVariantName");

        if (await HandleHttpResponseMessage(allVariantsMessage))
            return RedirectToAction("GetList", ControllerName, new { PageIndex = pageIndex });

        List<VariantResponseViewModel>? allVariants = JsonConvert.DeserializeObject<
            List<VariantResponseViewModel>>(await allVariantsMessage.Content.ReadAsStringAsync());

        ViewBag.AllVariants = allVariants;

        HttpResponseMessage existVariantProductsMessage = await client.GetAsync(
            $"{ApiUri}VariantProducts/OData?$filter=productId eq {id}");

        if (await HandleHttpResponseMessage(existVariantProductsMessage))
            return RedirectToAction("GetList", ControllerName, new { PageIndex = pageIndex });

        List<VariantProductViewModel> existVariantProducts = JsonConvert.DeserializeObject<
            List<VariantProductViewModel>>(await existVariantProductsMessage.Content.ReadAsStringAsync())!;

        ViewBag.ExistVariantIds = existVariantProducts.Select(x => x.VariantId).ToArray();

        return View();
    }
    [HttpPost]
    public async Task<IActionResult> VariantOperations(int productId, int[] variantIds, int pageIndex)
    {
        HttpClient client = CreateClientAndFillAuthenticationHeaderValueWithAccessToken();

        HttpResponseMessage existVariantProductsMessage = await client.GetAsync(
            $"{ApiUri}VariantProducts/OData?$filter=productId eq {productId}");

        if (await HandleHttpResponseMessage(existVariantProductsMessage))
            return RedirectToAction("GetList", ControllerName, new { PageIndex = pageIndex });

        List<VariantProductViewModel> existVariantProducts = JsonConvert.DeserializeObject<
            List<VariantProductViewModel>>(await existVariantProductsMessage.Content.ReadAsStringAsync())!;

        int[] existVariantIds = existVariantProducts.Select(x => x.VariantId).ToArray();

        int[] addedVariantIds = variantIds.Except(existVariantIds).ToArray();

        if (addedVariantIds.Length > 0)
        {
            AddRangeVariantProductsModel addRangeVariantProductsModel = new()
            {
                ProductId = productId,
                VariantIds = addedVariantIds
            };

            HttpResponseMessage addRangeVariantProductsMessage = await client.PostAsJsonAsync(
                $"{ApiUri}VariantProducts/AddRange", addRangeVariantProductsModel);

            if (await HandleHttpResponseMessage(addRangeVariantProductsMessage))
                return RedirectToAction("GetList", ControllerName, new { PageIndex = pageIndex });
        }

        int[] deletedVariantIds = existVariantIds.Except(variantIds).ToArray();

        if (deletedVariantIds.Length > 0)
        {
            int[] deletedVariantProductIds = existVariantProducts.Where(x => deletedVariantIds
                .Contains(x.VariantId)).Select(x => x.Id).ToArray();

            DeleteRangeVariantProductModel deleteRangeVariantProductModel = new()
            { Ids = deletedVariantProductIds };

            HttpResponseMessage deleteRangeVariantProductsMessage = await client.PostAsJsonAsync(
                $"{ApiUri}VariantProducts/DeleteRange", deleteRangeVariantProductModel);

            if (await HandleHttpResponseMessage(deleteRangeVariantProductsMessage))
                return RedirectToAction("GetList", ControllerName, new { PageIndex = pageIndex });
        }
        return RedirectToAction("GetList", ControllerName, new { PageIndex = pageIndex });
    }
}
