namespace Aika;

/// <summary>
/// Defines a handler for processing commands of a specific type.
/// Handlers are responsible for executing business logic associated with commands.
/// </summary>
/// <typeparam name="TCommand">
/// The type of command this handler processes. Must implement <see cref="ICommand{TResponse}"/>.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of response produced after handling the command.
/// </typeparam>
public interface ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    /// <summary>
    /// Asynchronously processes the specified command.
    /// </summary>
    /// <param name="command">The command instance to handle.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken);
}

