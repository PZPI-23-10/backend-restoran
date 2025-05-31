using backend_restoran.Persistence.Models;

namespace backend_restoran.Features.Tags;

public record GetTagsResponse(List<Tag> Tags, List<Cuisine> Cuisines, List<DressCode> DressCodes);