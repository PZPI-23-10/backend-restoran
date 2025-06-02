using System.ComponentModel.DataAnnotations.Schema;

namespace backend_restoran.Persistence.Models;

public class Reservation : BaseEntity
{
  public int PeopleCount { get; set; }
  public DateTime StartDate { get; set; }
  public DateTime EndDate { get; set; }
  [ForeignKey(nameof(Table))] public Guid TableId { get; set; }

  public Table Table;

  [ForeignKey(nameof(User))] public Guid UserId { get; set; }

  public User User;
}