﻿namespace ECommerceAdminUI.Models;

public class CategoryRequestAddModel
{
    public string SingleCategoryName { get; set; }
    public string FullCategoryName { get; set; }
    public bool IsProductCategorization { get; set; }
    public string TopCategoryName { get; set; }
    public int TopCategoryId { get; set; }
}


