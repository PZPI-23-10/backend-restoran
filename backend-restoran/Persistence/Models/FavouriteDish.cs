using System.ComponentModel.DataAnnotations.Schema;

namespace backend_restoran.Persistence.Models;

public class FavouriteDish : BaseEntity
{
  [ForeignKey(nameof(User))] public Guid UserId { get; set; }
  public User User;

  [ForeignKey(nameof(Dish))] public Guid DishId { get; set; }
  public Dish Dish;
}