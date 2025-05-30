using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend_restoran.Persistence.Models;

public class DishTag : BaseEntity
{
  [ForeignKey(nameof(Dish))] public Guid DishId { get; set; }
  public Dish Dish;

  [ForeignKey(nameof(Tag))] public Guid TagId { get; set; }
  [JsonInclude] public Tag Tag;
}