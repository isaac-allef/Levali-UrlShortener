namespace Levali.Core;

public interface IPublisher : IDisposable
{
    public Task PublishAsync<T>(string channel, T value) where T : notnull;
}
