using Microsoft.Extensions.DependencyInjection;

namespace Aika;

public interface IPlugin
{
    Task InitAsync(IServiceCollection services, CancellationToken cancellationToken);
    Task RunAsync(CancellationToken cancellationToken);
    Task StopAsyc(CancellationToken cancellationToken);
}
