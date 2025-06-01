namespace backend_restoran.Features.Users;

public record EditUserRequest(
        string FirstName,
        string LastName,
        string MiddleName,
        string Email,
        string Street,
        string City
    );