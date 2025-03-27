namespace ECommerceAdminUI.Models;

public class FoodResponseViewModel
{
    public int Id { get; set; }
    public string StorageCondition { get; set; }
    public string? PreparationTechnique { get; set; }
    public DateTime ExpiryDate { get; set; }
    public int ProductId { get; set; }
}
