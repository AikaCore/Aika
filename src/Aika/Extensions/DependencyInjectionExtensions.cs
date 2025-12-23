using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Aika;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddAikaHandlersFromAssemblyContains<TMarker>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient)
        => services.AddAikaHandlersFromAssembly(typeof(TMarker).Assembly, lifetime);

    public static IServiceCollection AddAikaHandlersFromAssembly(this IServiceCollection services, Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        services.AddHandlers(
            assembly,
            typeof(IEventHandler<>),
            (_, implementatoinType) => new ServiceDescriptor(implementatoinType, implementatoinType, lifetime)
        );

        services.AddHandlers(
            assembly,
            typeof(ICommandHandler<,>),
            (interfaceType, implementatoinType) => new ServiceDescriptor(interfaceType, implementatoinType, lifetime)
        );

        return services;
    }

    private static IServiceCollection AddHandlers(
        this IServiceCollection services, 
        Assembly assembly, 
        Type handlerInterfaceType,
        Func<Type, Type, ServiceDescriptor> descriptorFactory)
    {
        var handlerTypes = assembly.GetTypes()
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

        foreach (var handlerType in handlerTypes)
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

                var descriptor = descriptorFactory(genericInterface, handlerType);
                services.Add(descriptor);
            }
        }

        return services;
    }
}
