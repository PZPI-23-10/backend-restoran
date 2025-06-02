namespace backend_restoran.Chat.Models;

public class ActiveChatSession
{
  public string SessionId { get; } = Guid.NewGuid().ToString();
  public Guid RestaurantId { get; set; }
  public Guid UserId { get; set; }
  public Guid ModeratorId { get; set; }
  public List<ChatMessage> Messages { get; } = [];
  public DateTime CreatedAt { get; } = DateTime.UtcNow;
  public DateTime LastActivity { get; set; } = DateTime.UtcNow;
}