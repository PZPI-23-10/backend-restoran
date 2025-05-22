using System.Security.Claims;
using backend_restoran.Extensions;
using backend_restoran.Features.Restaurants;
using backend_restoran.Persistence;
using backend_restoran.Persistence.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace backend_restoran.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RestaurantController(DataContext dataContext) : ControllerBase
{
  [HttpPost]
  [Route("Create")]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  public async Task<IActionResult> CreateRestaurant([FromBody] CreateRestaurantRequest request)
  {
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrEmpty(userId))
      return Unauthorized("User ID not found in token.");

    var user = dataContext.Users.FirstOrDefault(x => x.Id == Guid.Parse(userId));

    if (user == null)
      return NotFound("User not found.");

    var moderators = request.ModeratorIds
      .Select(moderatorId => dataContext.Users.FirstOrDefault(x => x.Id.ToString() == moderatorId))
      .OfType<User>()
      .ToList();

    var tags = request.Tags
      .Select(tagName => dataContext.Tags.FirstOrDefault(x => x.Name == tagName))
      .OfType<Tag>()
      .ToList();

    var restaurant = new Restaurant
    {
      Name = request.Name,
      Description = request.Description,
      Address = request.Address,
      PhotoUrl = request.PhotoUrl,
      Email = request.Email,
      Layout = request.RestaurantLayout,
      Moderators = moderators,
      Tags = tags,
      City = request.City,
      Region = request.Region
    };

    var restaurantLayout = restaurant.Layout.FromJson<RestaurantLayout>();

    for (var index = 0; index < restaurantLayout.Layout.Count; index++)
    {
      var item = restaurantLayout.Layout[index];

      if (item.Type is LayoutItemType.MultipleSit or LayoutItemType.DoubleSit or LayoutItemType.ComfortableSit)
        item.Id = index;
    }

    await dataContext.Restaurants.AddAsync(restaurant);
    await dataContext.SaveChangesAsync();

    return Ok();
  }

  [HttpGet]
  public async Task<IActionResult> GetRestaurants()
  {
    var restaurants = await dataContext.Restaurants
      .Include(x => x.Moderators)
      .Include(x => x.Tags)
      .ToListAsync();

    return Ok(restaurants);
  }
}