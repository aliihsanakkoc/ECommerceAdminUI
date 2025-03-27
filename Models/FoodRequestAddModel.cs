namespace ECommerceAdminUI.Models;

public class FoodRequestAddModel
{
    public string StorageCondition { get; set; }
    public string? PreparationTechnique { get; set; }
    public DateTime ExpiryDate { get; set; }
    public int ProductId { get; set; }
}
