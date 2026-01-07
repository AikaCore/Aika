namespace Aika;

/// <summary>
/// Provides methods for registering event and command handlers in the application.
/// Allows registration by generic type or by runtime Type.
/// </summary>
public interface IHandlerRegistrator
{
    /// <summary>
    /// Registers an event handler for the specified event type.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event to handle, must implement <see cref="IEvent"/>.</typeparam>
    /// <returns>The current registrator for fluent configuration.</returns>
    IHandlerRegistrator RegisterEventHandler<TEvent, THandler>() where TEvent : IEvent, new();

    /// <summary>
    /// Registers an event handler for the specified event type using runtime type.
    /// </summary>
    /// <param name="eventType">The type of the event to handle.</param>
    /// <returns>The current registrator for fluent configuration.</returns>
    IHandlerRegistrator RegisterEventHandler(Type eventType, Type handlerType);

    /// <summary>
    /// Registers a command handler for the specified command and response types.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command to handle, must implement <see cref="ICommand{TResponse}"/>.</typeparam>
    /// <typeparam name="TResponse">The type of the response produced by the handler.</typeparam>
    /// <returns>The current registrator for fluent configuration.</returns>
    IHandlerRegistrator RegisterCommandHandler<TCommand, TResponse>() where TCommand : ICommand<TResponse>, new();

    /// <summary>
    /// Registers a command handler for the specified command and response types using runtime types.
    /// </summary>
    /// <param name="commandType">The type of the command to handle.</param>
    /// <param name="responseType">The type of the response produced by the handler.</param>
    /// <returns>The current registrator for fluent configuration.</returns>
    IHandlerRegistrator RegisterCommandHandler(Type commandType, Type responseType);
}

