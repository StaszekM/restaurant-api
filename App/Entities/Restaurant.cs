namespace RestaurantApi.Entities;

public class Restaurant
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public bool HasDelivery { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactNumber { get; set; }
    public int? CreatedById { get; set; }
    public virtual User? CreatedBy { get; set; } = null!;
    public int AddressId { get; set; }
    public virtual Address Address { get; set; } = null!;
    public virtual List<Dish> Dishes { get; set; } = null!;
}