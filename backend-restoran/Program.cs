using backend_restoran.Extensions;
using backend_restoran.Persistence;

namespace backend_restoran;

public static class Program
{
  public static void Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddAuthorization();
    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.ConfigureDatabase(builder.Configuration);

    var app = builder.Build();

    var serviceScope = app.Services.CreateScope();
    var dataContext = serviceScope.ServiceProvider.GetRequiredService<DataContext>();
    dataContext.Database.EnsureCreated();

    if (app.Environment.IsDevelopment())
    {
      app.UseSwagger();
      app.UseSwaggerUI();
    }

    app.UseAuthorization();
    app.UseErrorHandler();
    app.MapControllers();
    
    app.Run();
  }
}