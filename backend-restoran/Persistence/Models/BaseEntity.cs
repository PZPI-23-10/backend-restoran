using System.ComponentModel.DataAnnotations;

namespace backend_restoran.Persistence.Models;

public class BaseEntity
{
  [Key] public Guid Id { get; set; }
  public DateTime DateCreated { get; set; }
  public DateTime DateUpdated { get; set; }
}