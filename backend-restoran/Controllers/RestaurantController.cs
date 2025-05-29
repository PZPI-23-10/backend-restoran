using System.Security.Claims;
using backend_restoran.Extensions;
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
public class RestaurantController(DataContext dataContext) : ControllerBase
{
  [HttpGet]
  public async Task<IActionResult> GetRestaurants()
  {
    var restaurants = await dataContext.Restaurants
      .Include(r => r.Owner)
      .Include(r => r.Cuisines).ThenInclude(rc => rc.Cuisine)
      .Include(r => r.Tags).ThenInclude(rt => rt.Tag)
      .Include(r => r.Moderators).ThenInclude(rm => rm.User)
      .Include(r => r.Dishes).ThenInclude(d => d.Tags).ThenInclude(dt => dt.Tag)
      .Include(r => r.Schedule)
      .ToListAsync();

    return Ok(restaurants);
  }

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

    var restaurant = new Restaurant
    {
      Name = request.Name,
      City = request.City,
      Region = request.Region,
      Street = request.Street,
      Description = request.Description,
      Email = request.Email,
      PhotoUrl = request.PhotoUrl,
      Organization = request.Organization,
      Latitude = request.Latitude,
      Longitude = request.Longitude,
      Layout = request.Layout.ToJson(),
      OwnerId = Guid.Parse(userId),
    };

    await AddRestaurantCuisines(request, restaurant);
    await AddRestaurantTags(request, restaurant);
    await AddRestaurantModerators(request, restaurant);
    await AddDishes(request, restaurant);
    AddSchedule(request, restaurant);

    user.RestaurantsOwned.Add(restaurant);
    
    await dataContext.Restaurants.AddAsync(restaurant);
    await dataContext.SaveChangesAsync();

    return Ok();
  }

  private static void AddSchedule(CreateRestaurantRequest request, Restaurant restaurant)
  {
    var schedules = request.Schedule.Select(scheduleDto => new Schedule
      {
        Day = scheduleDto.Day,
        IsDayOff = scheduleDto.IsDayOff,
        Open = scheduleDto.Open,
        Close = scheduleDto.Close,
        RestaurantId = restaurant.Id,
      })
      .ToList();

    restaurant.Schedule = schedules;
  }

  private async Task AddDishes(CreateRestaurantRequest request, Restaurant restaurant)
  {
    var dishes = new List<Dish>();
    foreach (var dishDto in request.Dishes)
    {
      var dish = new Dish
      {
        Title = dishDto.Name,
        PhotoUrl = dishDto.PhotoUrl,
        Ingredients = dishDto.Ingredients,
        Price = dishDto.Price,
        Weight = dishDto.Weight,
        RestaurantId = restaurant.Id,
      };

      var dishTags = new List<DishTag>();
      foreach (var tagName in dishDto.Tags.Where(tagName => !string.IsNullOrWhiteSpace(tagName)))
      {
        var tag = await dataContext.Tags
          .FirstOrDefaultAsync(t => t.Name == tagName);

        if (tag == null)
        {
          tag = new Tag
          {
            Name = tagName,
          };
          dataContext.Tags.Add(tag);
        }

        dishTags.Add(new DishTag
        {
          DishId = dish.Id,
          TagId = tag.Id
        });
      }

      dish.Tags = dishTags;
      dishes.Add(dish);
    }

    restaurant.Dishes = dishes;
  }

  private async Task AddRestaurantModerators(CreateRestaurantRequest request, Restaurant restaurant)
  {
    var moderators = new List<RestaurantModerator>();
    foreach (var email in request.ModeratorEmails.Where(email => !string.IsNullOrWhiteSpace(email)))
    {
      var moderator = await dataContext.Users
        .FirstOrDefaultAsync(u => u.Email == email);

      if (moderator == null)
        continue;

      moderators.Add(new RestaurantModerator
      {
        RestaurantId = restaurant.Id,
        UserId = moderator.Id
      });
    }

    restaurant.Moderators = moderators;
  }

  private async Task AddRestaurantTags(CreateRestaurantRequest request, Restaurant restaurant)
  {
    var tags = new List<RestaurantTag>();
    foreach (var tagName in request.Tags.Where(tagName => !string.IsNullOrWhiteSpace(tagName)))
    {
      var tag = await dataContext.Tags
        .FirstOrDefaultAsync(t => t.Name == tagName);
      if (tag == null)
      {
        tag = new Tag
        {
          Name = tagName,
        };
        dataContext.Tags.Add(tag);
      }

      tags.Add(new RestaurantTag
      {
        RestaurantId = restaurant.Id,
        TagId = tag.Id
      });
    }

    restaurant.Tags = tags;
  }

  private async Task AddRestaurantCuisines(CreateRestaurantRequest request, Restaurant restaurant)
  {
    var cuisines = new List<RestaurantCuisine>();
    foreach (var cuisineName in request.Cuisine.Where(cuisineName => !string.IsNullOrWhiteSpace(cuisineName)))
    {
      var cuisine = await dataContext.Cuisines
        .FirstOrDefaultAsync(c => c.Name == cuisineName);

      if (cuisine == null)
      {
        cuisine = new Cuisine
        {
          Name = cuisineName,
        };

        dataContext.Cuisines.Add(cuisine);
      }

      cuisines.Add(new RestaurantCuisine
      {
        RestaurantId = restaurant.Id,
        CuisineId = cuisine.Id
      });
    }

    restaurant.Cuisines = cuisines;
  }

  [HttpDelete] [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  public async Task<IActionResult> DeleteRestaurant([FromBody] DeleteRestaurantRequest request)
  {
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrEmpty(userId))
      return Unauthorized("User ID not found in token.");

    var user = await dataContext.Users.FirstOrDefaultAsync(x => x.Id == Guid.Parse(userId));

    if (user == null)
      return NotFound("User not found.");
    
    var restaurant = await dataContext.Restaurants.FirstOrDefaultAsync(x => x.Id == Guid.Parse(request.RestaurantId));

    if (restaurant == null)
    {
      return NotFound("Restaurant not found.");
    }
    
    if (restaurant.OwnerId != Guid.Parse(userId))
    {
      return BadRequest("User does not have permission to delete restaurant.");
    }
    
    dataContext.Restaurants.Remove(restaurant);
    await dataContext.SaveChangesAsync();
    return Ok();
  }
  
  [HttpPost] [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  public async Task<IActionResult> EditRestaurant([FromBody] EditingRestaurantRequest request)
  {
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrEmpty(userId))
      return Unauthorized("User ID not found in token.");

    var user = await dataContext.Users.FirstOrDefaultAsync(x => x.Id == Guid.Parse(userId));

    if (user == null)
      return NotFound("User not found.");
    
    var restaurant = await dataContext.Restaurants.FirstOrDefaultAsync(x => x.Id == Guid.Parse(request.RestaurantId));

    if (restaurant == null)
    {
      return NotFound("Restaurant not found.");
    }
    
    if (restaurant.OwnerId != Guid.Parse(userId))
    {
      return BadRequest("User does not have permission to delete restaurant.");
    }

    restaurant.Name = request.Name;
    restaurant.City = request.City;
    restaurant.Region = request.Region;
    restaurant.Street = request.Street;
    restaurant.Description = request.Description;
    restaurant.Email = request.Email;
    restaurant.PhotoUrl = request.PhotoUrl;
    restaurant.Organization = request.Organization;
    restaurant.Latitude = request.Latitude;
    restaurant.Longitude = request.Longitude;
    restaurant.Layout = request.Layout.ToJson();
    
    await EditingRestaurantCuisines(request, restaurant);
    await EditingRestaurantTags(request, restaurant);
    await UpdateRestaurantModerators(request, restaurant);
    await UpdateDishes(request, restaurant);
    UpdateSchedule(request, restaurant);
    
    await dataContext.SaveChangesAsync();
    return Ok();
  }

  private async Task EditingRestaurantCuisines(EditingRestaurantRequest request, Restaurant restaurant)
  {
    dataContext.RestaurantCuisines.RemoveRange(restaurant.Cuisines);
    
    var cuisines = new List<RestaurantCuisine>();
    foreach (var cuisineName in request.Cuisine.Where(cuisineName => !string.IsNullOrWhiteSpace(cuisineName)))
    {
      var cuisine = await dataContext.Cuisines.FirstOrDefaultAsync(c => c.Name == cuisineName);

      if (cuisine == null)
      {
        cuisine = new Cuisine { Name = cuisineName };
        dataContext.Cuisines.Add(cuisine);
      }
      
      cuisines.Add(new RestaurantCuisine
      {
        RestaurantId = restaurant.Id,
        CuisineId = cuisine.Id
      });
    }
    
    restaurant.Cuisines = cuisines;
  }
  
  private async Task EditingRestaurantTags(EditingRestaurantRequest request, Restaurant restaurant)
  {
    dataContext.RestaurantTags.RemoveRange(restaurant.Tags);
    
    var tags = new List<RestaurantTag>();
    foreach (var tagName in request.Tags.Where(tagName => !string.IsNullOrWhiteSpace(tagName)))
    {
      var tag = await dataContext.Tags.FirstOrDefaultAsync(tag => tag.Name == tagName);

      if (tag == null)
      {
        tag = new Tag() { Name = tagName };
        dataContext.Tags.Add(tag);
      }
      
      tags.Add(new RestaurantTag
      {
        RestaurantId = restaurant.Id,
        TagId = tag.Id
      });
    }
    
    restaurant.Tags = tags;
  }
  
  
  private async Task UpdateRestaurantModerators(EditingRestaurantRequest request, Restaurant restaurant)
  {
    dataContext.RestaurantModerators.RemoveRange(restaurant.Moderators);
    
    var moderators = new List<RestaurantModerator>();
    foreach (var email in request.ModeratorEmails.Where(e => !string.IsNullOrWhiteSpace(e)))
    {
      var moderator = await dataContext.Users
        .FirstOrDefaultAsync(u => u.Email == email);

      if (moderator == null) continue;

      moderators.Add(new RestaurantModerator
      {
        RestaurantId = restaurant.Id,
        UserId = moderator.Id
      });
    }
    
    restaurant.Moderators = moderators;
  }
  
  private async Task UpdateDishes(EditingRestaurantRequest request, Restaurant restaurant)
  {
    foreach (var dish in restaurant.Dishes)
    {
      dataContext.DishTags.RemoveRange(dish.Tags);
    }
    dataContext.Dishes.RemoveRange(restaurant.Dishes);

    var dishes = new List<Dish>();
    foreach (var dishDto in request.Dishes)
    {
      var dish = new Dish
      {
        Title = dishDto.Name,
        PhotoUrl = dishDto.PhotoUrl,
        Ingredients = dishDto.Ingredients,
        Price = dishDto.Price,
        Weight = dishDto.Weight,
        RestaurantId = restaurant.Id,
      };

      var dishTags = new List<DishTag>();
      foreach (var tagName in dishDto.Tags.Where(t => !string.IsNullOrWhiteSpace(t)))
      {
        var tag = await dataContext.Tags
          .FirstOrDefaultAsync(t => t.Name == tagName);

        if (tag == null)
        {
          tag = new Tag { Name = tagName };
          dataContext.Tags.Add(tag);
        }

        dishTags.Add(new DishTag
        {
          DishId = dish.Id,
          TagId = tag.Id
        });
      }

      dish.Tags = dishTags;
      dishes.Add(dish);
    }
    
    restaurant.Dishes = dishes;
  }

  private void UpdateSchedule(EditingRestaurantRequest request, Restaurant restaurant)
  {
    dataContext.Schedules.RemoveRange(restaurant.Schedule);
    
    var schedules = request.Schedule.Select(s => new Schedule
    {
      Day = s.Day,
      IsDayOff = s.IsDayOff,
      Open = s.Open,
      Close = s.Close,
      RestaurantId = restaurant.Id,
    }).ToList();
    
    restaurant.Schedule = schedules;
  }
  
}