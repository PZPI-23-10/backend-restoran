using System.ComponentModel.DataAnnotations.Schema;

namespace backend_restoran.Persistence.Models;

public class Table : BaseEntity
{
  public int TableNumber { get; set; }
  public bool IsTaken { get; set; }

  public List<Reservation> Orders = [];
  [ForeignKey(nameof(Restaurant))] public Guid RestaurantId { get; set; }

  public Restaurant Restaurant;
}