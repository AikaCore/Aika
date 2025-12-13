namespace Aika;

/// <summary>
/// Configures execution behavior and routing options for commands sent through the message bus.
/// Provides control over timeouts, retry logic, and plugin targeting.
/// </summary>
public sealed class CommandOptions
{
    /// <summary>
    /// Default timeout duration for command execution in seconds.
    /// </summary>
    public const int DefaultTimeoutSeconds = 30;

    /// <summary>
    /// Default maximum number of retry attempts for failed commands.
    /// </summary>
    public const int DefaultMaxRetryCount = 5;

    /// <summary>
    /// Maximum time allowed for command execution before timing out.
    /// Defaults to <see cref="DefaultTimeoutSeconds"/> seconds.
    /// </summary>
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(DefaultTimeoutSeconds);

    /// <summary>
    /// Maximum number of retry attempts if command execution fails.
    /// Defaults to <see cref="DefaultMaxRetryCount"/>.
    /// </summary>
    public int MaxRetryCount { get; init; } = DefaultMaxRetryCount;

    /// <summary>
    /// Optional identifier of the specific plugin that should handle this command.
    /// When null, the command is routed to any available handler.
    /// </summary>
    public string? TargetPluginId { get; init; }
}

