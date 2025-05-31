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
    List<string> Cuisine,
    List<string> Tags,
    List<string> ModeratorEmails,
    List<EditDishDto> Dishes,
    List<EditLayoutItem> Layout,
    List<EditScheduleItemDto> Schedule
);

public record EditDishDto(
    string PhotoUrl,
    string Name,
    string Ingredients,
    int Price,
    int Weight,
    List<string> Tags
);

public record EditLayoutItem(
    int X,
    int Y,
    int TypeId,
    int Id,
    int Rotation,
    int Floor
);

public record EditScheduleItemDto(
    string Day,
    bool IsDayOff,
    string? Open = null,
    string? Close = null
);

public enum EditObjectType
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