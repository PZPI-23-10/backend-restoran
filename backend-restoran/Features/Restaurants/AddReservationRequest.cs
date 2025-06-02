namespace backend_restoran.Features.Restaurants;

public record AddReservationRequest(
  int PeopleCount,
  DateTime Date,
  Guid RestaurantId
);

public record AddReservationResponse(
  int TableNumber,
  int PeopleCount,
  DateTime Date,
  Guid RestaurantId
);