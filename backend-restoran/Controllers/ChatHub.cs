using System.Security.Claims;
using backend_restoran.Chat;
using backend_restoran.Chat.Models;
using backend_restoran.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace backend_restoran.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ChatHub(ChatSessionStore sessionStore, DataContext dataContext) : Hub
{
  public async Task<string> CreateChatSession(string restaurantId, string moderatorId)
  {
    var clientId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrEmpty(clientId))
      throw new InvalidOperationException("User is not authenticated.");

    var restaurantGuid = Guid.Parse(restaurantId);
    var clientGuid = Guid.Parse(clientId);
    var moderatorGuid = Guid.Parse(moderatorId);

    var restaurant = await dataContext.Restaurants
      .Include(restaurant => restaurant.Moderators)
      .FirstOrDefaultAsync(x => x.Id == restaurantGuid);

    if (restaurant == null)
      throw new InvalidOperationException("Restaurant not found.");

    var client = await dataContext.Users.FirstOrDefaultAsync(x => x.Id == clientGuid);
    var moderator = restaurant.Moderators.FirstOrDefault(x => x.UserId == moderatorGuid);

    if (client == null)
      throw new InvalidOperationException("Client not found.");

    if (moderator == null)
      throw new InvalidOperationException("Moderator not found in restaurant.");

    var session = sessionStore.CreateSession(restaurantGuid, clientGuid, moderatorGuid);
    return session.SessionId;
  }

  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  public List<ActiveChatSession> GetModeratorChatSessions()
  {
    var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrEmpty(userId))
      throw new InvalidOperationException("User is not authenticated.");

    return sessionStore.GetSessionsByModerator(userId);
  }

  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  public List<ActiveChatSession> GetUserChatSessions()
  {
    var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrEmpty(userId))
      throw new InvalidOperationException("User is not authenticated.");

    return sessionStore.GetSessionsByUser(userId);
  }

  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  public async Task JoinChatSession(string sessionId)
  {
    if (sessionStore.TryGetSession(sessionId, out var session))
    {
      await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
      await Clients.Caller.SendAsync("ReceiveHistory", session.Messages);
    }
    else
    {
      await Clients.Caller.SendAsync("SessionExpired");
    }
  }

  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  public async Task SendMessage(string sessionId, string text)
  {
    if (sessionStore.TryGetSession(sessionId, out var session))
    {
      var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

      var user = await dataContext.Users
        .FirstOrDefaultAsync(x => x.Id == Guid.Parse(userId));

      var message = new ChatMessage
      {
        SenderId = userId,
        SenderName = user.Email,
        Text = text
      };

      sessionStore.AddMessage(sessionId, message);

      await Clients.Group(sessionId).SendAsync("ReceiveMessage", message);
    }
  }
}