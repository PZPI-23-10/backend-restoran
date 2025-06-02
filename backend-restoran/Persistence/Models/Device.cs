using System.ComponentModel.DataAnnotations.Schema;

namespace backend_restoran.Persistence.Models;

public class Device : BaseEntity
{
  public string DeviceToken { get; set; }

  [ForeignKey(nameof(User))] public Guid UserId { get; set; }
  public User User;
}