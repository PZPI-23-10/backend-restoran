namespace backend_restoran.Features.Users;
public class LoginWithGoogleRequest
{
    public string GoogleToken { get; set; }
    public bool RememberMe { get; set; }
}