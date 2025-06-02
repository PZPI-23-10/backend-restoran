using System.Collections.Concurrent;
using backend_restoran.Chat.Models;

namespace backend_restoran.Chat;

public class ChatSessionStore
{
  private readonly ConcurrentDictionary<string, ActiveChatSession> _sessions = new();
  private readonly Timer _cleanupTimer;

  public ChatSessionStore()
  {
    _cleanupTimer = new Timer(_ => CleanupInactiveSessions(), null,
      TimeSpan.FromMinutes(30),
      TimeSpan.FromMinutes(30));
  }

  public ActiveChatSession CreateSession(Guid restaurantId, Guid userId, Guid moderatorId)
  {
    var session = new ActiveChatSession
    {
      RestaurantId = restaurantId,
      UserId = userId,
      ModeratorId = moderatorId
    };

    _sessions.TryAdd(session.SessionId, session);
    return session;
  }

  public bool TryGetSession(string sessionId, out ActiveChatSession session)
  {
    return _sessions.TryGetValue(sessionId, out session);
  }

  public void AddMessage(string sessionId, ChatMessage message)
  {
    if (_sessions.TryGetValue(sessionId, out var session))
    {
      session.Messages.Add(message);
      session.LastActivity = DateTime.UtcNow;
    }
  }

  private void CleanupInactiveSessions()
  {
    var cutoff = DateTime.UtcNow.AddHours(-2);
    var inactive = _sessions.Where(s => s.Value.LastActivity < cutoff).ToList();

    foreach (var session in inactive)
    {
      _sessions.TryRemove(session.Key, out _);
    }
  }

  public List<ActiveChatSession> GetSessionsByUser(string userId)
  {
    return _sessions.Values
      .Where(s => s.UserId.ToString() == userId)
      .ToList();
  }

  public List<ActiveChatSession> GetSessionsByModerator(string userId)
  {
    return _sessions.Values
      .Where(s => s.ModeratorId.ToString() == userId)
      .ToList();
  }
}