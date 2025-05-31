using System.ComponentModel.DataAnnotations.Schema;

namespace backend_restoran.Persistence.Models;

public class RestaurantDressCode : BaseEntity
{
  [ForeignKey(nameof(Restaurant))] public Guid RestaurantId { get; set; }
  public Restaurant Restaurant;

  [ForeignKey(nameof(DressCode))] public Guid DressCodeId { get; set; }
  public DressCode DressCode;
}