using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Aika;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddAikaHandlersFromAssemblyContains<TMarker>(this IServiceCollection services)
        => services.AddAikaHandlersFromAssembly(typeof(TMarker).Assembly);

    public static IServiceCollection AddAikaHandlersFromAssembly(this IServiceCollection services, Assembly assembly)
    {
        services.AddHandlers(assembly, typeof(IEventHandler<>));
        services.AddHandlers(assembly, typeof(ICommandHandler<,>));

        return services;
    }

    private static IServiceCollection AddHandlers(this  IServiceCollection services, Assembly assembly, Type handlerInterfaceType)
    {
        var eventHandlerTypes = assembly.GetTypes()
            .Where(t => t.IsAbstract is false 
                && t.IsInterface is false 
                && t.IsGenericTypeDefinition is false
                && t.IsClass
            )
            .Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType 
                    && i.GetGenericTypeDefinition() == handlerInterfaceType
                )
            );

        foreach (var handlerType in eventHandlerTypes)
        {
            var interfaces = handlerType.GetInterfaces()
                .Where(i => i.IsGenericType 
                    && i.GetGenericTypeDefinition() == handlerInterfaceType
                );

            foreach (var @interface in interfaces)
            {
                var genericTypes = @interface
                    .GetGenericArguments()
                    ?? throw new InvalidOperationException($"Failed to register handlers for assembly {assembly.GetName()}");

                var genericInterface = handlerInterfaceType
                    .MakeGenericType(genericTypes)
                    ?? throw new InvalidOperationException($"Failed to register handlers for assembly {assembly.GetName()}");

                services.AddTransient(genericInterface, handlerType);
            }
        }

        return services;
    }
}
