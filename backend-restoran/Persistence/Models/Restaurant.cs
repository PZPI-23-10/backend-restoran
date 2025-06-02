using System.ComponentModel.DataAnnotations.Schema;

namespace backend_restoran.Persistence.Models;

public class Restaurant : BaseEntity
{
  public string Name { get; set; }
  public string Description { get; set; }
  public string City { get; set; }
  public string Region { get; set; }
  public string Street { get; set; }
  public string Email { get; set; }
  public string Layout { get; set; }
  public string Organization { get; set; }
  public string PhotoUrl { get; set; }
  public decimal Latitude { get; set; }
  public decimal Longitude { get; set; }
  public bool HasParking { get; set; }
  public bool Accessible { get; set; }
  public List<RestaurantModerator> Moderators { get; set; } = [];
  public List<RestaurantTag> Tags { get; set; } = [];
  public List<Dish> Dishes { get; set; } = [];
  public List<Schedule> Schedule { get; set; } = [];
  public List<RestaurantCuisine> Cuisines { get; set; } = [];
  public List<RestaurantDressCode> DressCodes { get; set; } = [];
  public List<RestaurantPhoto> Photos { get; set; } = [];
  public List<Review> Reviews { get; set; } = [];
  public List<Table> Tables { get; set; } = [];
  [ForeignKey(nameof(User))] public Guid UserId { get; set; }

  public User User;
}