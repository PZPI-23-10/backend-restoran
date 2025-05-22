namespace backend_restoran.Persistence.Models;

public class Restaurant : BaseEntity
{
  public string Name { get; set; }
  public string Description { get; set; }
  public string City { get; set; }
  public string Region { get; set; }
  public string Address { get; set; }
  public string PhotoUrl { get; set; }
  public string Email { get; set; }
  public string Layout { get; set; }
  public List<User> Moderators { get; set; } = [];
  public List<Tag> Tags { get; set; } = [];
}