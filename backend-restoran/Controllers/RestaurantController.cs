using System.Security.Claims;
using backend_restoran.Extensions;
using backend_restoran.Features;
using backend_restoran.Features.Restaurants;
using backend_restoran.Persistence;
using backend_restoran.Persistence.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LayoutItem = backend_restoran.Features.Restaurants.LayoutItem;

namespace backend_restoran.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RestaurantController(DataContext dataContext) : ControllerBase
{
  [HttpPost]
  [Route("moderators/random")]
  public async Task<IActionResult> GetRandomModerator([FromBody] GetRandomModeratorRequest request)
  {
    if (request.RestaurantId == Guid.Empty)
      return BadRequest("Restaurant ID is required.");

    var moderators = await dataContext.RestaurantModerators
      .Where(r => r.RestaurantId == request.RestaurantId)
      .Include(x => x.User)
      .ToListAsync();

    if (moderators.Count == 0)
      return NotFound("No moderators found for this restaurant.");

    var random = new Random();
    var randomIndex = random.Next(moderators.Count);
    var moderator = moderators[randomIndex];

    return Ok(new GetRandomModeratorResponse(moderator.UserId, $"{moderator.User.LastName} {moderator.User.FirstName}",
      moderator.User.Email));
  }


  [HttpPost]
  [Route("Get")]
  public async Task<IActionResult> GetRestaurant([FromBody] GetRestaurantRequest request)
  {
    var id = request.RestaurantId;

    if (string.IsNullOrEmpty(id))
      return BadRequest("Restaurant ID is required.");

    if (!Guid.TryParse(id, out var restaurantId))
      return BadRequest("Invalid Restaurant ID format.");

    var restaurant = await dataContext.Restaurants
      .Include(r => r.User)
      .Include(r => r.Cuisines).ThenInclude(rc => rc.Cuisine)
      .Include(r => r.Tags).ThenInclude(rt => rt.Tag)
      .Include(r => r.Moderators).ThenInclude(rm => rm.User)
      .Include(r => r.Dishes)
      .Include(r => r.Schedule)
      .Include(r => r.Reviews)
      .Include(r => r.Photos)
      .Include(r => r.DressCodes).ThenInclude(rd => rd.DressCode)
      .FirstOrDefaultAsync(r => r.Id == restaurantId);

    if (restaurant == null)
      return NotFound("Restaurant not found.");

    return Ok(restaurant);
  }

  [HttpGet]
  public async Task<IActionResult> GetRestaurants([FromQuery] int page = 1, [FromQuery] int pageSize = 2)
  {
    var restaurants = await dataContext.Restaurants
      .AsNoTracking()
      .Include(r => r.Cuisines).ThenInclude(rc => rc.Cuisine)
      .Include(r => r.Tags).ThenInclude(rt => rt.Tag)
      .Include(r => r.Dishes)
      .Include(r => r.Schedule)
      .Include(r => r.Reviews)
      .Include(r => r.DressCodes).ThenInclude(x => x.DressCode)
      .Skip((page - 1) * pageSize)
      .Take(pageSize)
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
      PhotoUrl = request.PhotoUrl,
      Email = request.Email,
      Organization = request.Organization,
      Latitude = request.Latitude,
      Longitude = request.Longitude,
      Layout = request.Layout.ToJson(),
      UserId = Guid.Parse(userId),
      HasParking = request.HasParking,
      Accessible = request.Accessible,
    };

    AddTables(request.Layout, restaurant);
    await AddRestaurantPhotos(request.Gallery, restaurant);
    await AddRestaurantDressCodes(request.DressCode, restaurant);
    await AddRestaurantCuisines(request.Cuisine, restaurant);
    await AddRestaurantTags(request.Tags, restaurant);
    await AddRestaurantModerators(request.ModeratorEmails, restaurant);
    await AddDishes(request.Dishes, restaurant);
    AddSchedule(request.Schedule, restaurant);

    user.RestaurantsOwned.Add(restaurant);

    await dataContext.Restaurants.AddAsync(restaurant);
    await dataContext.SaveChangesAsync();

    return Ok();
  }

  private void AddTables(List<LayoutItem> layout, Restaurant restaurant)
  {
    foreach (var layoutItem in layout)
    {
      var isTable = layoutItem.TypeId
        is (int)ObjectType.SeatingForTwo
        or (int)ObjectType.SeatingForMany
        or (int)ObjectType.TableWithSofa;

      if (!isTable)
        continue;

      var table = new Table
      {
        TableNumber = layoutItem.Id,
        RestaurantId = restaurant.Id,
      };

      restaurant.Tables.Add(table);
    }
  }

  private async Task AddRestaurantPhotos(List<string> requestGallery, Restaurant restaurant)
  {
    var photos = new List<RestaurantPhoto>();
    foreach (var photoUrl in requestGallery.Where(url => !string.IsNullOrWhiteSpace(url)))
    {
      photos.Add(new RestaurantPhoto
      {
        RestaurantId = restaurant.Id,
        Url = photoUrl
      });
    }

    if (photos.Count != 0)
      restaurant.Photos = photos;
  }

  private async Task AddRestaurantDressCodes(List<string> requestDressCode, Restaurant restaurant)
  {
    var dressCodes = new List<RestaurantDressCode>();
    foreach (var tagName in requestDressCode.Where(tagName => !string.IsNullOrWhiteSpace(tagName)))
    {
      var dressCode = await dataContext.DressCodes
        .FirstOrDefaultAsync(t => t.Name == tagName);

      if (dressCode == null)
        continue;

      dressCodes.Add(new RestaurantDressCode()
      {
        RestaurantId = restaurant.Id,
        DressCodeId = dressCode.Id
      });
    }

    if (dressCodes.Count != 0)
      restaurant.DressCodes = dressCodes;
  }

  private static void AddSchedule(List<ScheduleItemDto> requestSchedule, Restaurant restaurant)
  {
    var schedules = requestSchedule.Select(scheduleDto => new Schedule
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

  private async Task AddDishes(List<DishDto> requestDishes, Restaurant restaurant)
  {
    var dishes = new List<Dish>();
    foreach (var dishDto in requestDishes)
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

      dishes.Add(dish);
    }

    if (dishes.Count != 0)
      restaurant.Dishes = dishes;
  }

  private async Task AddRestaurantModerators(List<string> requestModeratorEmails, Restaurant restaurant)
  {
    var moderators = new List<RestaurantModerator>();
    foreach (var email in requestModeratorEmails.Where(email => !string.IsNullOrWhiteSpace(email)))
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

    if (moderators.Count != 0)
      restaurant.Moderators = moderators;
  }

  private async Task AddRestaurantTags(List<string> requestTags, Restaurant restaurant)
  {
    var tags = new List<RestaurantTag>();
    foreach (var tagName in requestTags.Where(tagName => !string.IsNullOrWhiteSpace(tagName)))
    {
      var tag = await dataContext.Tags
        .FirstOrDefaultAsync(t => t.Name == tagName);

      if (tag == null)
        continue;

      tags.Add(new RestaurantTag
      {
        RestaurantId = restaurant.Id,
        TagId = tag.Id
      });
    }

    if (tags.Count != 0)
      restaurant.Tags = tags;
  }

  private async Task AddRestaurantCuisines(List<string> requestCuisine, Restaurant restaurant)
  {
    var cuisines = new List<RestaurantCuisine>();
    foreach (var cuisineName in requestCuisine.Where(cuisineName => !string.IsNullOrWhiteSpace(cuisineName)))
    {
      var cuisine = await dataContext.Cuisines
        .FirstOrDefaultAsync(c => c.Name == cuisineName);

      if (cuisine == null)
        continue;

      cuisines.Add(new RestaurantCuisine
      {
        RestaurantId = restaurant.Id,
        CuisineId = cuisine.Id
      });
    }

    if (cuisines.Count != 0)
      restaurant.Cuisines = cuisines;
  }

  [HttpDelete]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

    if (restaurant.UserId != Guid.Parse(userId))
    {
      return BadRequest("User does not have permission to delete restaurant.");
    }

    dataContext.Restaurants.Remove(restaurant);
    await dataContext.SaveChangesAsync();
    return Ok();
  }

  [HttpPost]
  [Route("Editing")]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  public async Task<IActionResult> EditRestaurant([FromBody] EditingRestaurantRequest request)
  {
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrEmpty(userId))
      return Unauthorized("User ID not found in token.");

    var user = await dataContext.Users.FirstOrDefaultAsync(x => x.Id == Guid.Parse(userId));

    if (user == null)
      return NotFound("User not found.");

    var restaurant = await dataContext.Restaurants
      .Include(r => r.User)
      .Include(r => r.Cuisines).ThenInclude(rc => rc.Cuisine)
      .Include(r => r.Tags).ThenInclude(rt => rt.Tag)
      .Include(r => r.Moderators).ThenInclude(rm => rm.User)
      .Include(r => r.Dishes)
      .Include(r => r.Schedule)
      .Include(r => r.Reviews)
      .Include(r => r.Photos)
      .Include(r => r.DressCodes).Include(restaurant => restaurant.Tables)
      .FirstOrDefaultAsync(x => x.Id == Guid.Parse(request.RestaurantId));

    if (restaurant == null)
      return NotFound("Restaurant not found.");

    var isOwner = restaurant.UserId == Guid.Parse(userId);
    var isModerator = restaurant.Moderators.Any(m => m.UserId == Guid.Parse(userId));

    if (!isOwner && !isModerator)
      return BadRequest("User does not have permission to edit restaurant.");

    restaurant.Name = request.Name;
    restaurant.City = request.City;
    restaurant.PhotoUrl = request.PhotoUrl;
    restaurant.Region = request.Region;
    restaurant.Street = request.Street;
    restaurant.Description = request.Description;
    restaurant.Email = request.Email;
    restaurant.Organization = request.Organization;
    restaurant.Latitude = request.Latitude;
    restaurant.Longitude = request.Longitude;
    restaurant.Layout = request.Layout.ToJson();
    restaurant.HasParking = request.HasParking;
    restaurant.Accessible = request.Accessible;

    dataContext.Tables.RemoveRange(restaurant.Tables);
    dataContext.RestaurantCuisines.RemoveRange(restaurant.Cuisines);
    dataContext.RestaurantTags.RemoveRange(restaurant.Tags);
    dataContext.RestaurantModerators.RemoveRange(restaurant.Moderators);
    dataContext.Dishes.RemoveRange(restaurant.Dishes);
    dataContext.Schedules.RemoveRange(restaurant.Schedule);
    dataContext.RestaurantPhotos.RemoveRange(restaurant.Photos);
    dataContext.RestaurantDressCodes.RemoveRange(restaurant.DressCodes);

    AddTables(request.Layout, restaurant);
    await AddRestaurantCuisines(request.Cuisine, restaurant);
    await AddRestaurantTags(request.Tags, restaurant);
    await AddRestaurantModerators(request.ModeratorEmails, restaurant);
    await AddDishes(request.Dishes, restaurant);
    AddSchedule(request.Schedule, restaurant);
    await AddRestaurantPhotos(request.Gallery, restaurant);

    await AddRestaurantDressCodes(request.DressCode, restaurant);

    await dataContext.SaveChangesAsync();
    return Ok();
  }
}