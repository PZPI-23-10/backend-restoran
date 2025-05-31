using backend_restoran.Features.Reviews;
using backend_restoran.Persistence;
using backend_restoran.Persistence.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_restoran.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController(DataContext dataContext) : ControllerBase
{
  [HttpPost]
  public async Task<IActionResult> CreateReview([FromBody] CreateReviewRequest request)
  {
    if (request.Rating is < 1 or > 5)
      return BadRequest("Rating must be between 1 and 5.");

    if (string.IsNullOrEmpty(request.Comment))
      return BadRequest("Comment cannot be empty.");

    var restaurantId = Guid.Parse(request.RestaurantId);
    var userId = Guid.Parse(request.UserId);

    var review = new Review
    {
      Rating = request.Rating,
      Comment = request.Comment,
      RestaurantId = restaurantId,
      UserId = userId
    };

    await dataContext.Reviews.AddAsync(review);
    await dataContext.SaveChangesAsync();

    return Ok();
  }

  [HttpGet]
  [Route("user/{userId}")]
  public async Task<IActionResult> GetReviewsByUserId(string userId)
  {
    var reviews = await dataContext.Reviews
      .Where(r => r.UserId == Guid.Parse(userId))
      .ToListAsync();

    if (reviews.Count == 0)
      return NotFound("No reviews found for the specified user.");

    return Ok(reviews);
  }

  [HttpGet]
  [Route("restaurant/{restaurantId}")]
  public async Task<IActionResult> GetReviewsByRestaurantId(string restaurantId)
  {
    var reviews = await dataContext.Reviews
      .Where(r => r.RestaurantId == Guid.Parse(restaurantId))
      .ToListAsync();

    if (reviews.Count == 0)
      return NotFound("No reviews found for the specified restaurant.");

    return Ok(reviews);
  }
}