namespace Aika;

/// <summary>
/// Provides a unified interface for publishing events and sending commands within the application.
/// Acts as the central mediator for intra-process message routing.
/// </summary>
public interface IMessageBus
{
    /// <summary>
    /// Asynchronously publishes an event to all registered handlers.
    /// Multiple handlers can subscribe to the same event type.
    /// </summary>
    /// <typeparam name="TEvent">The type of event to publish. Must implement <see cref="IEvent"/>.</typeparam>
    /// <param name="event">The event instance to publish.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous publish operation.</returns>
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
        where TEvent : IEvent;

    /// <summary>
    /// Asynchronously sends a command to its registered handler and returns the response.
    /// Each command type must have exactly one handler.
    /// </summary>
    /// <typeparam name="TCommand">The type of command to send. Must implement <see cref="ICommand{TResponse}"/>.</typeparam>
    /// <typeparam name="TResponse">The type of response expected from the command handler.</typeparam>
    /// <param name="command">The command instance to send.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, containing the handler's response.</returns>
    Task<TResponse> SendAsync<TCommand, TResponse>(TCommand command, CancellationToken cancellationToken)
        where TCommand : ICommand<TResponse>;
}