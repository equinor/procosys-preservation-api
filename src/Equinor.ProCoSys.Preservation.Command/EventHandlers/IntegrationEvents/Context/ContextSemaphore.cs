using System;
using System.Threading;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.Context;

public class ContextSemaphore : IContextSemaphore
{
    // The database context is not thread safe and should not be accessed concurrently.
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly TimeSpan _semaphoreTimeout = TimeSpan.FromSeconds(30);
    
    public async Task WaitAsync()
    {
        if (!await _semaphore.WaitAsync(_semaphoreTimeout))
        {
            throw new TimeoutException("CreateEventHelper timed out on waiting for available semaphore instance");
        }
    }

    public void Release() => _semaphore.Release();
}
