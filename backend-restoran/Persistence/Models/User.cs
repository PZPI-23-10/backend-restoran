namespace backend_restoran.Persistence.Models;

public class User : BaseEntity
{
  public string FirstName { get; set; }
  public string LastName { get; set; }
  public string MiddleName { get; set; }
  public string Email { get; set; }
  public string Password { get; set; }
  public string Address { get; set; }
  public List<Dish> FavoriteDishes { get; set; }
  public List<Restaurant> FavoriteRestaraunt { get; set; }
}