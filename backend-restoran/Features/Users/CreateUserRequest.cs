namespace backend_restoran.Features.Users;

public record CreateUserRequest(
  string FirstName,
  string LastName,
  string MiddleName,
  string Email,
  string Password,
  string Address);