using backend_restoran.Persistence;

namespace backend_restoran;

public static class Program
{
  public static void Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.ConfigureDatabase(builder.Configuration);
    
    var app = builder.Build();
    
    var serviceScope = app.Services.CreateScope();
    var dataContext = serviceScope.ServiceProvider.GetRequiredService<DataContext>();
    dataContext.Database.EnsureCreated();
    
    app.Run();
  }
}