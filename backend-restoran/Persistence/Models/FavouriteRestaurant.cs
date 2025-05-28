using System.ComponentModel.DataAnnotations.Schema;

namespace backend_restoran.Persistence.Models;

public class FavouriteRestaurant : BaseEntity
{
  [ForeignKey(nameof(User))] public Guid UserId { get; set; }
  public User User;

  [ForeignKey(nameof(Restaurant))] public Guid RestaurantId { get; set; }
  public Restaurant Restaurant;
}