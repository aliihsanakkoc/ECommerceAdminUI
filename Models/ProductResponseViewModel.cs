namespace ECommerceAdminUI.Models;

public class ProductResponseViewModel
{
    public int Id { get; set; }
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public string? ProductDescription { get; set; }
    public string? ProductImageUrl { get; set; }
    public decimal ProductPrice { get; set; }
    public bool IsAddToCart { get; set; }
    public string ProductType { get; set; }

}
