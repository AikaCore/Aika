namespace Aika;

/// <summary>
/// Represents an application event that signals something has occurred in the system.
/// Events are published to notify interested subscribers about state changes or important occurrences.
/// </summary>
public interface IEvent
{
    /// <summary>
    /// String identifier of the event type used for routing and subscription matching.
    /// </summary>
    string EventType { get; }
}
