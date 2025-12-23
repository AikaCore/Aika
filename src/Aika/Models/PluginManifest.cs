using Aika.Helpers;
using NuGet.Versioning;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Aika;

/// <summary>
/// Contains metadata and configuration information for an Aika plugin.
/// Describes plugin identity, versioning, runtime requirements, capabilities, and dependencies.
/// </summary>
public class PluginManifest
{
    /// <summary>
    /// Unique identifier for the plugin.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Semantic version of the plugin.
    /// </summary>
    [JsonConverter(typeof(NuGetVersionJsonConverter))]
    public required NuGetVersion Version { get; init; }

    /// <summary>
    /// Composite identifier combining plugin ID and version in the format "Id@Major.Minor.Build".
    /// </summary>
    public string FullId => $"{Id}@{Version}";

    /// <summary>
    /// Human-readable display name of the plugin.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Optional description of the plugin's purpose and functionality.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Optional author or organization that created the plugin.
    /// </summary>
    public string? Author { get; init; }

    /// <summary>
    /// Optional license under which the plugin is distributed.
    /// </summary>
    public string? License { get; init; }

    /// <summary>
    /// Version of the Aika host application required to run this plugin.
    /// </summary>
    [JsonConverter(typeof(VersionRangeJsonConverter))]
    public VersionRange? RequiredHostVersion { get; init; }


    /// <summary>
    /// Array of capability identifiers that this plugin provides or requires.
    /// Used for feature discovery and compatibility checking.
    /// </summary>
    public string[] Capabilities { get; init; } = [];

    /// <summary>
    /// Optional array of other plugins that this plugin depends on.
    /// Dependencies must be loaded before this plugin can initialize.
    /// </summary>
    public PluginDependence[]? Dependencies { get; init; }

    [JsonInclude]
    [JsonPropertyName("PluginConfiguration")]
    private JsonElement? _pluginConfiguration;
    private object? _cachedConfiguration;

    /// <summary>
    /// Retrieves and deserializes the plugin configuration as the specified type.
    /// Configuration is loaded from the PluginConfiguration section in the manifest and cached after first access.
    /// </summary>
    /// <typeparam name="TConfiguration">The target type to deserialize the configuration into. Must be a reference type.</typeparam>
    /// <returns>The deserialized configuration object.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when PluginConfiguration is null in the manifest or deserialization fails.
    /// </exception>
    public TConfiguration GetConfiguration<TConfiguration>()
        where TConfiguration : class
    {
        if (_cachedConfiguration is TConfiguration cached)
            return cached;

        if (_pluginConfiguration.HasValue is false)
            throw new InvalidOperationException("Failed to retrieve configuration: 'PluginConfiguration' is null. Ensure that the configuration object is defined in the manifest file");

        var configuration = _pluginConfiguration.Value
            .Deserialize<TConfiguration>()
            ?? throw new InvalidOperationException($"Failed to deserialize 'PluginConfiguration' to type '{typeof(TConfiguration).Name}'");

        _cachedConfiguration = configuration;
        _pluginConfiguration = null;
        return configuration;
    }

}
