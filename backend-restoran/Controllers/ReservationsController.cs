using System.Security.Claims;
using backend_restoran.Features.Restaurants;
using backend_restoran.Persistence;
using backend_restoran.Persistence.Models;
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
      StartDate = request.Date.ToUniversalTime(),
      UserId = userGuid,
      TableId = table.Id
    };

    await dataContext.Reservations.AddAsync(reservation);

    table.IsTaken = true;

    await dataContext.SaveChangesAsync();

    return Ok(new AddReservationResponse(table.TableNumber, reservation.PeopleCount, reservation.StartDate, table.RestaurantId));
  }

  [HttpGet]
  [Route("ReservationsByUser")]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  public async Task<IActionResult> GetReservationsByUser()
  {
    var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrEmpty(userId))
      return BadRequest("User ID is required.");

    if (!Guid.TryParse(userId, out var userGuid))
      return BadRequest("Invalid User ID format.");

    var reservations = await dataContext.Reservations
      .Where(r => r.UserId == userGuid)
      .Include(r => r.Table)
      .Select(r => new
      {
        ReservationId = r.Id,
        PeopleCount = r.PeopleCount,
        Date = r.StartDate,
        TableNumber = r.Table.TableNumber,
        RestaurantName = r.Table.Restaurant.Name,
        RestaurantPhoto = r.Table.Restaurant.PhotoUrl,
        RestaurantCity = r.Table.Restaurant.City,
        RestaurantRegion = r.Table.Restaurant.Region,
        RestaurantStreet = r.Table.Restaurant.Street,
      })
      .ToListAsync();

    return Ok(reservations);
  }
  
  [HttpDelete("{reservationId}")]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  public async Task<IActionResult> CancelReservation(Guid reservationId)
  {
    var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrEmpty(userId))
      return BadRequest("User ID is required.");

    if (!Guid.TryParse(userId, out var userGuid))
      return BadRequest("Invalid User ID format.");

    var reservation = await dataContext.Reservations
      .Include(r => r.Table) 
      .FirstOrDefaultAsync(r => r.Id == reservationId && r.UserId == userGuid);

    if (reservation == null)
      return NotFound("Reservation not found or you don't have permission to cancel it.");

    reservation.Table.IsTaken = false;

    dataContext.Reservations.Remove(reservation);

    await dataContext.SaveChangesAsync();

    return Ok("Reservation canceled successfully.");
  }
}