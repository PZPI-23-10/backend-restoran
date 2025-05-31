using backend_restoran.Features.Tags;
using backend_restoran.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace backend_restoran.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagController(DataContext dataContext) : ControllerBase
{
  [HttpGet]
  public IActionResult GetTags()
  {
    var tags = dataContext.Tags.ToList();
    var cuisines = dataContext.Cuisines.ToList();
    var dressCodes = dataContext.DressCodes.ToList();

    if (cuisines.Count == 0 && tags.Count == 0 && dressCodes.Count == 0)
      return NotFound("No tags found.");

    return Ok(new GetTagsResponse(tags, cuisines, dressCodes));
  }
}