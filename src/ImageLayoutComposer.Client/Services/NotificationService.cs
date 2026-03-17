namespace ImageLayoutComposer.Client.Services;

public class NotificationService
{
    public event Action<string, string>? OnMessage;

    public void NotifyError(string message)
    {
        OnMessage?.Invoke(message, "alert-danger");
    }

    public void NotifySuccess(string message)
    {
        OnMessage?.Invoke(message, "alert-success");
    }
}
