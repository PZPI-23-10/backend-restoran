namespace backend_restoran.Features.Restaurants;

public record CreateRestaurantRequest(
  string Name,
  string Description,
  string PhotoUrl,
  string Address,
  string Email,
  string KitchenType,
  string[] Tags,
  string[] ModeratorIds,
  string RestaurantLayout);