namespace Aika;

public sealed class CommandOptions
{
    public const int DefaultTimeoutSeconds = 30;
    public const int DefaultMaxRetryCount = 5;

    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(DefaultTimeoutSeconds);
    public int MaxRetryCount { get; init; } = DefaultMaxRetryCount;
    public string? TargetPluginId { get; init; }
}
