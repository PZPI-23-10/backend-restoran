using System.Text;
using backend_restoran.Extensions;
using backend_restoran.Persistence;
using backend_restoran.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace backend_restoran;

public static class Program
{
  public static void Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddAuthentication(cfg =>
    {
      cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
      cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      cfg.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(x =>
    {
      x.RequireHttpsMetadata = false;
      x.SaveToken = false;
      x.TokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
          Encoding.UTF8
            .GetBytes(builder.Configuration.GetSection("JwtSettings")["AccessSecretKey"]!)
        ),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
      };
    });


    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
      options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
      {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
      });

      options.AddSecurityRequirement(new OpenApiSecurityRequirement
      {
        {
          new OpenApiSecurityScheme
          {
            Reference = new OpenApiReference
            {
              Type = ReferenceType.SecurityScheme,
              Id = "Bearer"
            }
          },
          Array.Empty<string>()
        }
      });
    });

    builder.Services.ConfigureDatabase(builder.Configuration);
    builder.Services.AddSingleton<TokenService>();
    builder.Services.AddAuthorization();

    var app = builder.Build();

    var serviceScope = app.Services.CreateScope();
    var dataContext = serviceScope.ServiceProvider.GetRequiredService<DataContext>();
    dataContext.Database.EnsureCreated();

    if (app.Environment.IsDevelopment())
    {
      app.UseSwagger();
      app.UseSwaggerUI();
    }

    app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

    app.UseAuthorization();
    app.UseAuthentication();
    app.UseErrorHandler();
    app.MapControllers();

    app.Run();
  }
}