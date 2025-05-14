using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using backend_restoran.Features.Users;
using backend_restoran.Persistence;
using backend_restoran.Persistence.Models;
using backend_restoran.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend_restoran.Controllers;

[ApiController]
[Route("api/account/[controller]")]
public class UsersController(DataContext dataContext, TokenService tokenService) : ControllerBase
{
    [HttpPost]
    [Route("Register")]
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

        return Ok();
    }

    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserRequest request)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            return BadRequest("All fields are required.");

        var user = dataContext.Users.FirstOrDefault(u => u.Email == request.Email);
        if (user == null)
            return Unauthorized("User with this email does not exist.");

        var inputBytes = Encoding.UTF8.GetBytes(request.Password);
        var hashBytes = MD5.HashData(inputBytes);
        var hashedPassword = Convert.ToBase64String(hashBytes);

        if (user.Password != hashedPassword)
            return Unauthorized("Invalid Password.");

        var accessToken = tokenService.GenerateAccessToken(user.Id.ToString(), user.Email, request.RememberMe);

        return Ok(new LoginUserResponse(user.Id.ToString(), accessToken.TokenKey));
    }

    [HttpGet]
    [Route("Test")]
    [Authorize]
    public async Task<IActionResult> LoginTest()
    {
        return Ok(HttpContext.User.Claims);
    }
}