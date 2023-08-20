namespace Levali.Core;

public interface IDequeueer : IDisposable
{
    public Task<T?> DequeueAsync<T>(string queueName);
}
