namespace backend_restoran.Chat.Models;

public class ChatMessage
{
  public string SenderId { get; set; }
  public string SenderName { get; set; }
  public string Text { get; set; }
  public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}