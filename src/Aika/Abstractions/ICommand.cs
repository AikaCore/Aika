namespace Aika;

/// <summary>
/// Represents an application command that produces a response of the specified type.
/// Used as a contract for objects passed into the messagebus pipeline.
/// </summary>
/// <typeparam name="TResponse">
/// The type of the response produced by the handler of this command.
/// </typeparam>
public interface ICommand<TResponse>
{
    /// <summary>
    /// String identifier of the command type used for routing.
    /// </summary>
    string CommandType { get; }

    /// <summary>
    /// Identifier of the sender that initiated this command.
    /// Can be used for auditing, correlation, and reply routing.
    /// </summary>
    string SenderId { get; }
}