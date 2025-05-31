using System.ComponentModel.DataAnnotations.Schema;

namespace backend_restoran.Persistence.Models;

public class Dish : BaseEntity
{
  public string Title { get; set; }
  public string PhotoUrl { get; set; }
  public decimal Price { get; set; }
  public decimal Weight { get; set; }
  public string Ingredients { get; set; }

  [ForeignKey(nameof(Restaurant))] public Guid RestaurantId { get; set; }
  public Restaurant Restaurant;
}