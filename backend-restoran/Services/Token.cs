namespace backend_restoran.Services;

public class Token
{
    public string TokenKey { get; set; } = string.Empty;
    public DateTime Expires { get; set; }
}