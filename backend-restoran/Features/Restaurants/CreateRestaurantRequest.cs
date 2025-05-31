namespace backend_restoran.Features.Restaurants;

public record CreateRestaurantRequest(
  string Name,
  string City,
  string Region,
  string Street,
  string Description,
  decimal Latitude,
  decimal Longitude,
  bool HasParking,
  bool Accessible,
  List<string> DressCode,
  List<string> Gallery,
  List<string> Cuisine,
  List<string> Tags,
  List<string> ModeratorEmails,
  List<DishDto> Dishes,
  List<LayoutItem> Layout,
  List<ScheduleItemDto> Schedule,
  string Organization = "",
  string Email = "");

public record DishDto(
  string PhotoUrl,
  string Name,
  string Ingredients,
  int Price,
  int Weight
);

public record LayoutItem(
  int X,
  int Y,
  int TypeId,
  int Id,
  int Rotation,
  int Floor
);

public record ScheduleItemDto(
  string Day,
  bool IsDayOff,
  string? Open = null,
  string? Close = null
);

public enum ObjectType
{
  Wall = 1,
  Circle,
  Corner,
  Door,
  Window,
  SeatingForTwo,
  SeatingForMany,
  TableWithSofa,
  BarCounter,
  Stairs
};