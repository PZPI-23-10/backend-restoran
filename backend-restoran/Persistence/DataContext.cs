using backend_restoran.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_restoran.Persistence;

public class DataContext : DbContext
{
  public DataContext(DbContextOptions options) : base(options) { }

  public DbSet<User> Users { get; set; }
  public DbSet<Tag> Tags { get; set; }
  public DbSet<Dish> Dishes { get; set; }
  public DbSet<Cuisine> Cuisines { get; set; }
  public DbSet<DressCode> DressCodes { get; set; }
  public DbSet<Restaurant> Restaurants { get; set; }
  public DbSet<RestaurantCuisine> RestaurantCuisines { get; set; }
  public DbSet<RestaurantDressCode> RestaurantDressCodes { get; set; }
  public DbSet<RestaurantModerator> RestaurantModerators { get; set; }
  public DbSet<RestaurantTag> RestaurantTags { get; set; }
  public DbSet<Schedule> Schedules { get; set; }
  public DbSet<FavouriteDish> FavouriteDishes { get; set; }
  public DbSet<FavouriteRestaurant> FavouriteRestaurants { get; set; }
  public DbSet<RestaurantPhoto> RestaurantPhotos { get; set; }
  public DbSet<Review> Reviews { get; set; }

  public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    var now = DateTime.UtcNow;

    foreach (var changedEntity in ChangeTracker.Entries())
    {
      if (changedEntity.Entity is BaseEntity entity)
      {
        switch (changedEntity.State)
        {
          case EntityState.Added:
            entity.DateCreated = now;
            entity.DateUpdated = now;
            break;

          case EntityState.Modified:
            Entry(entity).Property(x => x.Id).IsModified = false;
            entity.DateUpdated = now;
            break;
        }
      }
    }

    return await base.SaveChangesAsync(cancellationToken);
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    ConfigureRestaurant(modelBuilder);
    ConfigureFavouriteItems(modelBuilder);
    ConfigureDishes(modelBuilder);
    ConfigureSchedule(modelBuilder);
    ConfigureRestaurantCuisine(modelBuilder);
    ConfigureRestaurantDressCodes(modelBuilder);
    ConfigureRestaurantPhotos(modelBuilder);
    ConfigureRestaurantModerator(modelBuilder);
    ConfigureRestaurantTag(modelBuilder);
    ConfigureReviews(modelBuilder);
  }

  private void ConfigureReviews(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Review>()
      .HasOne(r => r.Restaurant)
      .WithMany(r => r.Reviews)
      .HasForeignKey(r => r.RestaurantId)
      .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<Review>()
      .HasOne(r => r.User)
      .WithMany(u => u.Reviews)
      .HasForeignKey(r => r.UserId)
      .OnDelete(DeleteBehavior.Cascade);
  }

  private static void ConfigureRestaurantPhotos(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<RestaurantPhoto>()
      .HasOne(rp => rp.Restaurant)
      .WithMany(r => r.Photos)
      .HasForeignKey(rp => rp.RestaurantId)
      .OnDelete(DeleteBehavior.Cascade);
  }

  private static void ConfigureRestaurant(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Restaurant>()
      .HasOne(r => r.User)
      .WithMany(u => u.RestaurantsOwned)
      .HasForeignKey(r => r.UserId)
      .OnDelete(DeleteBehavior.Cascade);
  }

  private static void ConfigureSchedule(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Schedule>().HasOne(s => s.Restaurant)
      .WithMany(r => r.Schedule)
      .HasForeignKey(s => s.RestaurantId)
      .OnDelete(DeleteBehavior.Cascade);
  }

  private static void ConfigureRestaurantCuisine(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<RestaurantCuisine>(entity =>
    {
      entity.HasOne(rc => rc.Restaurant)
        .WithMany(r => r.Cuisines)
        .HasForeignKey(rc => rc.RestaurantId)
        .OnDelete(DeleteBehavior.Cascade);

      entity.HasOne(rc => rc.Cuisine)
        .WithMany()
        .HasForeignKey(rc => rc.CuisineId)
        .OnDelete(DeleteBehavior.Cascade);
    });
  }

  private static void ConfigureRestaurantDressCodes(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<RestaurantDressCode>(entity =>
    {
      entity.HasOne(rc => rc.Restaurant)
        .WithMany(r => r.DressCodes)
        .HasForeignKey(rc => rc.RestaurantId)
        .OnDelete(DeleteBehavior.Cascade);

      entity.HasOne(rc => rc.DressCode)
        .WithMany()
        .HasForeignKey(rc => rc.DressCodeId)
        .OnDelete(DeleteBehavior.Cascade);
    });
  }


  private static void ConfigureRestaurantModerator(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<RestaurantModerator>(entity =>
    {
      entity.HasOne(rm => rm.User)
        .WithMany()
        .HasForeignKey(rm => rm.UserId)
        .OnDelete(DeleteBehavior.Cascade);

      entity.HasOne(rm => rm.Restaurant)
        .WithMany(r => r.Moderators)
        .HasForeignKey(rm => rm.RestaurantId)
        .OnDelete(DeleteBehavior.Cascade);
    });
  }

  private static void ConfigureRestaurantTag(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<RestaurantTag>(entity =>
    {
      entity.HasOne(rt => rt.Restaurant)
        .WithMany(r => r.Tags)
        .HasForeignKey(rt => rt.RestaurantId)
        .OnDelete(DeleteBehavior.Cascade);

      entity.HasOne(rt => rt.Tag)
        .WithMany()
        .HasForeignKey(rt => rt.TagId)
        .OnDelete(DeleteBehavior.Cascade);
    });
  }

  private static void ConfigureDishes(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Dish>()
      .HasOne(d => d.Restaurant)
      .WithMany(r => r.Dishes)
      .HasForeignKey(d => d.RestaurantId)
      .OnDelete(DeleteBehavior.Cascade);
  }

  private static void ConfigureFavouriteItems(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<FavouriteDish>()
      .HasOne(favouriteItem => favouriteItem.Dish)
      .WithMany()
      .HasForeignKey(favouriteItem => favouriteItem.DishId)
      .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<FavouriteDish>()
      .HasOne(favouriteItem => favouriteItem.User)
      .WithMany(client => client.FavoriteDishes)
      .HasForeignKey(favouriteItem => favouriteItem.UserId)
      .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<FavouriteRestaurant>()
      .HasOne(favouriteItem => favouriteItem.Restaurant)
      .WithMany()
      .HasForeignKey(favouriteItem => favouriteItem.RestaurantId)
      .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<FavouriteRestaurant>()
      .HasOne(favouriteItem => favouriteItem.User)
      .WithMany(client => client.FavoriteRestaurants)
      .HasForeignKey(favouriteItem => favouriteItem.UserId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}