namespace Aika;

/// <summary>
/// Represents a plugin dependency requirement.
/// Specifies another plugin that must be loaded and available for the dependent plugin to function correctly.
/// </summary>
public sealed class PluginDependence
{
    /// <summary>
    /// Unique identifier of the required plugin.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Minimum version of the required plugin that satisfies this dependency.
    /// </summary>
    public required Version Version { get; set; }
}
