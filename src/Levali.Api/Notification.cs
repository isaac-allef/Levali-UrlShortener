namespace Levali.Api;

public sealed class Notification
{
    public List<string> Errors { get; private set; }

    public Notification()
    {
        Errors = new();
    }
    
    public void AddError(string messageError)
        => Errors.Add(messageError);

    public bool HasErrors()
        => Errors.Count > 0;
}
