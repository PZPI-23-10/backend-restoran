using System.ComponentModel.DataAnnotations.Schema;

namespace backend_restoran.Persistence.Models;

public class RestaurantModerator : BaseEntity
{
  [ForeignKey(nameof(User))] public Guid UserId { get; set; }
  public User User;

  [ForeignKey(nameof(Restaurant))] public Guid RestaurantId { get; set; }
  public Restaurant Restaurant;
}