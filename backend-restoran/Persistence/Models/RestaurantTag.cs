using System.ComponentModel.DataAnnotations.Schema;

namespace backend_restoran.Persistence.Models;

public class RestaurantTag : BaseEntity
{
  [ForeignKey(nameof(Restaurant))] public Guid RestaurantId { get; set; }
  public Restaurant Restaurant;

  [ForeignKey(nameof(Tag))] public Guid TagId { get; set; }
  public Tag Tag;
}