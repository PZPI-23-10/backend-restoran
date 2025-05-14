using Microsoft.EntityFrameworkCore;

namespace backend_restoran.Persistence;

public static class DatabaseExtensions
{
  public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
  {
    var dbPath = Path.Combine(AppContext.BaseDirectory, configuration.GetConnectionString("Database"));
    services.AddDbContext<DataContext>(options =>
      options.UseSqlite($"Data Source={dbPath}"));
  }
}