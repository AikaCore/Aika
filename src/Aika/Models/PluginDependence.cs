using Aika.Helpers;
using NuGet.Versioning;
using System.Text.Json.Serialization;

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
    public required string PluginId { get; set; }

    /// <summary>
    /// Minimum version of the required plugin that satisfies this dependency.
    /// </summary>
    [JsonConverter(typeof(VersionRangeJsonConverter))]
    public required VersionRange Version { get; set; }
}
