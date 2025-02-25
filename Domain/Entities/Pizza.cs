namespace Domain.Entities;

public class Pizza
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string ImageUrl { get; set; }
    public int CategoryId { get; set; } 
    public Category Category { get; set; }
}
