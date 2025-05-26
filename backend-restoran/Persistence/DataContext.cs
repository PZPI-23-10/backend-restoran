using backend_restoran.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_restoran.Persistence;

public class DataContext : DbContext
{
  public DataContext(DbContextOptions options) : base(options) { }

  public DbSet<User> Users { get; set; }
  public DbSet<Tag> Tags { get; set; }
    public DbSet<Dish> Dishes { get; set; }
    public DbSet<Restaurant> Restaurants { get; set; }
}