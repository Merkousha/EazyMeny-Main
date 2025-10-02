namespace EazyMenu.Application.Common.Models;

/// <summary>
/// DTO ساده برای اطلاعات پایه رستوران
/// </summary>
public class RestaurantBasicDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public bool IsActive { get; set; }
}
