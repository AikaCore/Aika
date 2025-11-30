namespace Aika;

public sealed class PluginManifest
{
    public required string Id { get; set; }
    public required Version Version { get; set; }
    public string FullName => $"{Id}@{Version.ToString(3)}";
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Author { get; set; }
    public string? License { get; set; }
    public Version? MinimumHostVersion { get; set; }
    public PluginRuntime? Runtime { get; set; }
    public string[] Capabilities { get; set; } = [];
    public PluginDependence[]? Dependences { get; set; }
}