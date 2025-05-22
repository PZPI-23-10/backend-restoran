using backend_restoran.Persistence.Models;

namespace backend_restoran.Features.Restaurants;

public record CreateRestaurantRequest(
  string Name,
  string Description,
  string PhotoUrl,
  string Address,
  string City,
  string Region,
  string Email,
  string KitchenType,
  string[] Tags,
  string[] ModeratorIds,
  string RestaurantLayout,
  DishDTO[] Dishes);