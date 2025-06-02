namespace backend_restoran.Features.Restaurants;

public record EditingRestaurantRequest(
  string RestaurantId,
  string Name,
  string City,
  string Region,
  string Street,
  string Description,
  string Email,
  string Organization,
  decimal Latitude,
  decimal Longitude,
  bool HasParking,
  bool Accessible,
  List<string> Cuisine,
  List<string> Tags,
  List<string> ModeratorEmails,
  List<string> DressCode,
  List<string> Gallery,
  List<DishDto> Dishes,
  List<LayoutItem> Layout,
  List<ScheduleItemDto> Schedule,
  string PhotoUrl = "");