using System.Security.Claims;
using backend_restoran.Features.Restaurants;
using backend_restoran.Features.Users;
using backend_restoran.Persistence;
using backend_restoran.Persistence.Models;
using backend_restoran.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_restoran.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController(DataContext dataContext) : ControllerBase
{
  [HttpPost]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  public async Task<IActionResult> CreateReservation([FromBody] AddReservationRequest request)
  {
    var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrEmpty(userId))
      return BadRequest("User ID is required.");

    if (!Guid.TryParse(userId, out var userGuid))
      return BadRequest("Invalid User ID format.");

    if (!await dataContext.Users.AnyAsync(user => user.Id == userGuid))
      return NotFound("User not found.");

    var table = await dataContext.Tables.FirstOrDefaultAsync(t =>
      t.RestaurantId == request.RestaurantId && !t.IsTaken);

    if (table == null)
      return NotFound("Table not found.");

    var reservation = new Reservation
    {
      PeopleCount = request.PeopleCount,
      Date = request.Date,
      UserId = userGuid,
      TableId = table.Id
    };

    await dataContext.Reservations.AddAsync(reservation);

    table.IsTaken = true;

    await dataContext.SaveChangesAsync();

    return Ok(userGuid);
  }
}