using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend_restoran.Persistence.Models;

public class RestaurantDressCode : BaseEntity
{
  [ForeignKey(nameof(Restaurant))] public Guid RestaurantId { get; set; }
  public Restaurant Restaurant;

  [ForeignKey(nameof(DressCode))] public Guid DressCodeId { get; set; }
  [JsonInclude] public DressCode DressCode;
}