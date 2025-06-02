namespace backend_restoran.Features.Restaurants;

public record AddReservationRequest(
    int PeopleCount,
    DateTime Date,
    Guid RestaurantId
        );