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
  
  public List<RestaurantModerator> Moderators = [];
  public List<RestaurantTag> Tags = [];
  public List<Dish> Dishes = [];
  public List<Schedule> Schedule = [];
  public List<RestaurantCuisine> Cuisines = [];

  [ForeignKey(nameof(User))] public Guid UserId { get; set; }
  public User User;
}