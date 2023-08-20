namespace Levali.Core;

public interface IEnqueueer : IDisposable
{
    public Task EnqueueAsync<T>(string queueName, T item) where T : notnull;
}
