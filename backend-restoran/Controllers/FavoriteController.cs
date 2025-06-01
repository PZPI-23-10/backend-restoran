using System.Security.Claims;
using backend_restoran.Features.Users;
using backend_restoran.Persistence;
using backend_restoran.Persistence.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_restoran.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FavoriteController(DataContext dataContext) : ControllerBase
{
  [HttpPost]
  [Route("Dish")]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  public async Task<IActionResult> AddDish(AddingFavoriteDishRequest request)
  {
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrEmpty(userId))
      return Unauthorized("User ID not found in token.");

    var user = dataContext.Users.FirstOrDefault(x => x.Id == Guid.Parse(userId));

    if (user == null)
      return NotFound("User not found.");

    var dishId = request.DishId;
    var dish = await dataContext.Dishes.FirstOrDefaultAsync(x => x.Id == Guid.Parse(dishId));

    if (dish == null)
      return NotFound("Dish not found.");

    user.FavoriteDishes.Add(new FavouriteDish
    {
      Dish = dish,
      User = user
    });

    await dataContext.SaveChangesAsync();

    return Ok();
  }


  [HttpPost]
  [Route("Restaurant")]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  public async Task<IActionResult> AddRestaurant(AddingFavoriteRestaurantRequest request)
  {
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrEmpty(userId))
      return Unauthorized("User ID not found in token.");

    var user = await dataContext.Users.FirstOrDefaultAsync(x => x.Id == Guid.Parse(userId));

    if (user == null)
      return NotFound("User not found.");

    var restaurantId = request.RestaurantId;
    var restaurant = await dataContext.Restaurants.FirstOrDefaultAsync(x => x.Id == Guid.Parse(restaurantId));

    if (restaurant == null)
      return NotFound("Restaurant not found.");

    user.FavoriteRestaurants.Add(new FavouriteRestaurant
    {
      Restaurant = restaurant,
      User = user
    });

    await dataContext.SaveChangesAsync();

    return Ok();
  }

  [HttpGet]
  [Route("Dish")]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  public async Task<IActionResult> GetDishes()
  {
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrEmpty(userId))
      return Unauthorized("User ID not found in token.");

    var user = dataContext.Users.FirstOrDefault(x => x.Id == Guid.Parse(userId));

    if (user == null)
      return NotFound("User not found.");

    var favoriteDishes = await dataContext.FavouriteDishes.Where(x => x.UserId == user.Id)
      .Include(x => x.Dish)
      .ToListAsync();

    if (favoriteDishes.Count == 0)
      return NotFound("No favorite dishes found for the user.");

    return Ok(favoriteDishes.Select(x => x.Dish));
  }

  [HttpGet]
  [Route("Restaurant")]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  public async Task<IActionResult> GetRestaurants()
  {
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrEmpty(userId))
      return Unauthorized("User ID not found in token.");

    var user = await dataContext.Users.FirstOrDefaultAsync(x => x.Id == Guid.Parse(userId));

    if (user == null)
      return NotFound("User not found.");

    var favoriteRestaurants = await dataContext.FavouriteRestaurants.Where(x => x.UserId == user.Id)
      .Include(x => x.Restaurant)
      .ToListAsync();

    if (favoriteRestaurants.Count == 0)
      return NotFound("No favorite restaurants found for the user.");

    return Ok(favoriteRestaurants.Select(x => x.Restaurant));
  }
}