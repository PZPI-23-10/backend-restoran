using backend_restoran.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_restoran.Persistence;

public class DataContext : DbContext
{
  public DataContext(DbContextOptions options) : base(options) { }
  
  public DbSet<User> Users { get; set; }
}