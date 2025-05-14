using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using backend_restoran.Features.Users;
using backend_restoran.Persistence;
using backend_restoran.Persistence.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend_restoran.Controllers;

[ApiController]
[Route("users")]
public class UsersController(DataContext dataContext) : ControllerBase
{
  [HttpPost]
  public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
  {
    if (string.IsNullOrEmpty(request.FirstName) || string.IsNullOrEmpty(request.MiddleName) ||
        string.IsNullOrEmpty(request.LastName) || string.IsNullOrEmpty(request.Email) ||
        string.IsNullOrEmpty(request.Password) || string.IsNullOrEmpty(request.Address))
    {
      return BadRequest("All fields are required.");
    }

    if (dataContext.Users.Any(u => u.Email == request.Email))
      return BadRequest("User with this email already exists.");

    if (request.Password.Length is < 5 or > 27)
      return BadRequest("Password must be between 5 and 20 characters.");

    if (!Regex.IsMatch(request.Password, @"^[a-zA-Z0-9!@#$%^&*]+$"))
      return BadRequest("Password can only contain letters, numbers, and special characters.");

    if (!Regex.IsMatch(request.Password, @"[!@#$%^&*]"))
      return BadRequest("Password must contain at least one special character.");

    var inputBytes = Encoding.UTF8.GetBytes(request.Password);
    var hashBytes = MD5.HashData(inputBytes);

    var user = new User
    {
      FirstName = request.FirstName,
      LastName = request.LastName,
      MiddleName = request.MiddleName,
      Email = request.Email,
      Password = Convert.ToBase64String(hashBytes),
      Address = request.Address
    };

    await dataContext.Users.AddAsync(user);
    await dataContext.SaveChangesAsync();

    return Ok(); //TODO: send auth token
  }
}