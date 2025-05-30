using System.ComponentModel.DataAnnotations.Schema;

namespace backend_restoran.Persistence.Models;

public class Restaurant : BaseEntity
{
  public string Name { get; set; }
  public string Description { get; set; }
  public string City { get; set; }
  public string Region { get; set; }
  public string Street { get; set; }
  public string PhotoUrl { get; set; }
  public string Email { get; set; }
  public string Layout { get; set; }
  public string Organization { get; set; }
  
  public decimal Latitude  { get; set; }
  public decimal Longitude { get; set; }
  
  public List<RestaurantModerator> Moderators { get; set; } = new();
  public List<RestaurantTag> Tags { get; set; } = new();
  public List<Dish> Dishes { get; set; } = new();
  public List<Schedule> Schedule { get; set; } = new();
  public List<RestaurantCuisine> Cuisines { get; set; } = new();

  [ForeignKey(nameof(User))] public Guid UserId { get; set; }
  public User User;
}