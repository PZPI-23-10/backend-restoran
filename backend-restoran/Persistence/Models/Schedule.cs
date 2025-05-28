using System.ComponentModel.DataAnnotations.Schema;

namespace backend_restoran.Persistence.Models;

public class Schedule : BaseEntity
{
  public string Day { get; set; }
  public bool IsDayOff { get; set; }
  public string? Open { get; set; }
  public string? Close { get; set; }

  [ForeignKey(nameof(Restaurant))] public Guid RestaurantId { get; set; }
  public Restaurant Restaurant;
}