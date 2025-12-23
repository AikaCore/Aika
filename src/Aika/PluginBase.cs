using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.Json;

namespace Aika;

/// <summary>
/// Base implementation of <see cref="IPlugin"/> providing common plugin lifecycle management.
/// Handles manifest loading, handler registration, and default lifecycle logging.
/// Derived plugins should inherit from this class to get standard plugin infrastructure.
/// </summary>
/// <param name="logger">Logger instance for plugin operations.</param>
public abstract class PluginBase(ILogger<PluginBase> logger) : IPlugin
{
    /// <summary>
    /// Standard filename for the plugin manifest file.
    /// </summary>
    public const string PluginManifestFileName = "plugin.json";

    /// <summary>
    /// Logger for plugin operations and diagnostics.
    /// </summary>
    protected readonly ILogger<PluginBase> Logger = logger;

    /// <summary>
    /// Initializes the plugin by registering command/event handlers.
    /// Scans the plugin assembly for handlers and automatically registers them in the DI container.
    /// </summary>
    /// <param name="services">The service collection to register plugin dependencies.</param>
    /// <param name="cancellationToken">Token to cancel the initialization operation.</param>
    /// <returns>A task representing the asynchronous initialization operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when manifest file is missing or invalid.</exception>
    public virtual async Task InitAsync(IServiceCollection services, IHandlerRegistrator handlerRegistrator, Assembly pluginAssembly, CancellationToken cancellationToken)
    {
        services.AddAikaHandlersFromAssembly(pluginAssembly);
    }

    protected async Task<PluginManifest> LoadManifestFromAssemblyAsync(Assembly assembly, CancellationToken cancellationToken)
    {
        var pluginDir = Path.GetDirectoryName(assembly.Location)
            ?? throw new InvalidOperationException("Failed to get plugin directory.");
        var pathToManifest = Path.Combine(pluginDir, PluginManifestFileName);

        if (File.Exists(pathToManifest) is false)
        {
            Logger.LogCritical("Failed to load plugin manifest by path '{Path}'. Ensure it in directory '{Dir}'", pathToManifest, pluginDir);
            throw new InvalidOperationException("Failed to load plugin manifest.");
        }

        var json = await File.ReadAllTextAsync(pathToManifest, cancellationToken);
        var manifest = JsonSerializer.Deserialize<PluginManifest>(json);
        if (manifest is not null)
            return manifest;

        Logger.LogCritical("Failed to load plugin manifest from '{Path}'. Make sure that the file has a valid JSON schema", pathToManifest);
        throw new InvalidOperationException("Failed to load plugin manifest.");
    }

    /// <summary>
    /// Starts the plugin's main execution logic.
    /// Default implementation do nothing. Override to add custom startup behavior.
    /// </summary>
    /// <param name="serviceProvider">A service provider for receiving registered services.</param>
    /// <param name="cancellationToken">Token to cancel the execution operation.</param>
    /// <returns>A completed task.</returns>
    public virtual Task RunAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
        => Task.CompletedTask;

    /// <summary>
    /// Stops the plugin and performs cleanup operations.
    /// Default implementation do nothing. Override to add custom shutdown behavior.
    /// </summary>
    /// <param name="serviceProvider">A service provider for receiving registered services.</param>
    /// <param name="cancellationToken">Token to cancel the stop operation.</param>
    /// <returns>A completed task.</returns>
    public virtual Task StopAsyc(IServiceProvider serviceProvider, CancellationToken cancellationToken)
        => Task.CompletedTask;
}