namespace backend_restoran.Persistence.Models;

public class Dish : BaseEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string PhotoUrl { get; set; }
    public decimal Price { get; set; }
    public decimal Quantity { get; set; }
    public List<string> Ingredients { get; set; }
    public List<string> Tag { get; set; }
}

public class DishDTO
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string PhotoUrl { get; set; }
    public decimal Price { get; set; }
    public decimal Quantity { get; set; }
    public List<string> Ingredients { get; set; }
    public List<string> Tag { get; set; }
}