using System.ComponentModel.DataAnnotations.Schema;

namespace backend_restoran.Persistence.Models;

public class Review : BaseEntity
{
  public int Rating { get; set; }
  public string Comment { get; set; }

  [ForeignKey(nameof(Restaurant))] public Guid RestaurantId { get; set; }
  public Restaurant Restaurant { get; set; }

  [ForeignKey(nameof(User))] public Guid UserId { get; set; }
  public User User { get; set; }
}