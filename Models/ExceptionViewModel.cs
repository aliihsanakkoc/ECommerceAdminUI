namespace ECommerceAdminUI.Models;

public class ExceptionViewModel
{
    public string Type { get; set; }
    public string Title { get; set; }
    public int Status { get; set; }
    public string Detail { get; set; }
    public Error[] Errors { get; set; }
}

public class Error
{
    public string Property { get; set; }
    public string[] Errors { get; set; }
}
