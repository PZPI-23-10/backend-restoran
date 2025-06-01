namespace backend_restoran.Persistence.Models;

public class User : BaseEntity
{
  public string FirstName { get; set; }
  public string? LastName { get; set; }
  public string? MiddleName { get; set; }
  public string Email { get; set; }
  public string? Password { get; set; }
  public string? City { get; set; }
  public string? Street { get; set; }
  public bool IsGoogleAuth { get; set; } = false;
  public List<FavouriteDish> FavoriteDishes { get; set; } = [];
  public List<FavouriteRestaurant> FavoriteRestaurants { get; set; } = [];
  public List<Restaurant> RestaurantsOwned { get; set; } = [];
  public List<RestaurantModerator> RestaurantsModerating { get; set; } = [];
  public List<Review> Reviews { get; set; } = [];
}