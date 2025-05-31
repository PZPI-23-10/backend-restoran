using System.ComponentModel.DataAnnotations.Schema;

namespace backend_restoran.Persistence.Models;

public class RestaurantPhoto : BaseEntity
{
  public string Url { get; set; }

  [ForeignKey(nameof(Restaurant))] public Guid RestaurantId { get; set; }
  public Restaurant Restaurant;
}