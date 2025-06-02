using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;

namespace backend_restoran.Services;

public class FirebaseService
{
  public FirebaseService()
  {
    var path = Path.Combine(AppContext.BaseDirectory, "restaurant-app-19cb8-5ecea50a869a.json");

    FirebaseApp.Create(new AppOptions
    {
      Credential = GoogleCredential.FromFile(path),
    });
  }

  public async Task SendNotificationAsync(string fcmToken, string title, string body, string imageUrl = "")
  {
    var message = new Message
    {
      Token = fcmToken,
      Notification = new Notification
      {
        Title = title,
        Body = body,
        ImageUrl = imageUrl
      }
    };

    var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
  }
}