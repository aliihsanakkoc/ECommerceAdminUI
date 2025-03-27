namespace ECommerceAdminUI.Models;

public class BookRequestAddModel
{
    public string Title { get; set; } 
    public string Author { get; set; } 
    public string ISBN { get; set; } 
    public string Publisher { get; set; } 
    public DateTime PublishedDate { get; set; }
    public int Edition { get; set; }
    public int ProductId { get; set; }
}
