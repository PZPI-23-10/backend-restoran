using System.ComponentModel.DataAnnotations.Schema;

namespace backend_restoran.Persistence.Models;

public class RestaurantCuisine : BaseEntity
{
  [ForeignKey(nameof(Restaurant))] public Guid RestaurantId { get; set; }
  public Restaurant Restaurant;

  [ForeignKey(nameof(Cuisine))] public Guid CuisineId { get; set; }
  public Cuisine Cuisine;
}