namespace backend_restoran.Features.Reviews;

public record CreateReviewRequest(
  int Rating,
  string Comment,
  string RestaurantId,
  string UserId);