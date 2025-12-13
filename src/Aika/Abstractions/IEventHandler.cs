namespace Aika;

/// <summary>
/// Defines a handler for processing events of a specific type.
/// Handlers are invoked when matching events are published through the event bus.
/// </summary>
/// <typeparam name="TEvent">
/// The type of event this handler processes. Must implement <see cref="IEvent"/>.
/// </typeparam>
public interface IEventHandler<TEvent>
    where TEvent : IEvent
{
    /// <summary>
    /// Asynchronously processes the specified event.
    /// </summary>
    /// <param name="event">The event instance to handle.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken);
}

