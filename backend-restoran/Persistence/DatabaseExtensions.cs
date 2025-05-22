using Microsoft.EntityFrameworkCore;

namespace backend_restoran.Persistence;

public static class DatabaseExtensions
{
  public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
  {
    var connectionString = configuration.GetConnectionString("Database");
    services.AddDbContext<DataContext>(options =>
      options.UseNpgsql(connectionString));
  }
}