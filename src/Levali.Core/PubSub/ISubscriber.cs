namespace Levali.Core;

public interface ISubscriber : IDisposable
{
    public Task SubscribeAsync<T>(string channel, Func<T, Task> run) where T : notnull;
}
