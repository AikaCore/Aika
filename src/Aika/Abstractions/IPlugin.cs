using Microsoft.Extensions.DependencyInjection;

namespace Aika;

/// <summary>
/// Defines the lifecycle contract for Aika plugins.
/// Plugins extend application functionality through initialization, execution, and cleanup phases.
/// </summary>
public interface IPlugin
{
    /// <summary>
    /// Asynchronously initializes the plugin and registers its services in the dependency injection container.
    /// Called once during application startup before <see cref="RunAsync"/>.
    /// </summary>
    /// <param name="services">The service collection to register plugin dependencies.</param>
    /// <param name="cancellationToken">Token to cancel the initialization operation.</param>
    /// <returns>A task representing the asynchronous initialization operation.</returns>
    Task InitAsync(IServiceCollection services, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously starts the plugin's main execution logic.
    /// Called after successful initialization to begin plugin operations.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the execution operation.</param>
    /// <returns>A task representing the asynchronous execution operation.</returns>
    Task RunAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously stops the plugin and performs cleanup operations.
    /// Called during application shutdown to gracefully terminate plugin activities.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the stop operation.</param>
    /// <returns>A task representing the asynchronous stop operation.</returns>
    Task StopAsyc(CancellationToken cancellationToken);
}

