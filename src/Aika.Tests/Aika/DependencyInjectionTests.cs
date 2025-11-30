using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Aika.Tests.Aika;

[TestFixture]
public class AddAikaHandlersFromAssemblyTests
{
    private IServiceCollection _services = null!;

    [SetUp]
    public void Setup()
    {
        _services = new ServiceCollection();
    }

    [Test]
    public void AddAikaHandlersFromAssembly_ShouldRegisterEventHandler()
    {
        // Arrange
        var assembly = typeof(TestEventHandler).Assembly;

        // Act
        _services.AddAikaHandlersFromAssembly(assembly);
        var provider = _services.BuildServiceProvider();

        // Assert
        var handlers = provider.GetServices<IEventHandler<TestEvent>>();
        Assert.That(handlers.Count(), Is.Not.Zero);
        Assert.That(handlers.Any(h => typeof(TestEventHandler) == h.GetType()));
    }

    [Test]
    public void AddAikaHandlersFromAssembly_ShouldRegisterCommandHandler()
    {
        // Arrange
        var assembly = typeof(TestCommandHandler).Assembly;

        // Act
        _services.AddAikaHandlersFromAssembly(assembly);
        var provider = _services.BuildServiceProvider();

        // Assert
        var handlers = provider.GetServices<ICommandHandler<TestCommand, TestResult>>();
        Assert.That(handlers.Count(), Is.Not.Zero);
        Assert.That(handlers.Any(h => typeof(TestCommandHandler) == h.GetType()));
    }

    [Test]
    public void AddAikaHandlersFromAssembly_ShouldRegisterBothHandlerTypes()
    {
        // Arrange
        var assembly = typeof(TestEventHandler).Assembly;

        // Act
        _services.AddAikaHandlersFromAssembly(assembly);

        // Assert
        Assert.That(_services, Has.Some.Matches<ServiceDescriptor>(d =>
            d.ServiceType == typeof(IEventHandler<TestEvent>) &&
            d.ImplementationType == typeof(TestEventHandler)));

        Assert.That(_services, Has.Some.Matches<ServiceDescriptor>(d =>
            d.ServiceType == typeof(ICommandHandler<TestCommand, TestResult>) &&
            d.ImplementationType == typeof(TestCommandHandler)));
    }

    [Test]
    public void AddAikaHandlersFromAssembly_ShouldRegisterAsTransient()
    {
        // Arrange
        var assembly = typeof(TestEventHandler).Assembly;

        // Act
        _services.AddAikaHandlersFromAssembly(assembly);

        // Assert
        var descriptors = _services.Where(d =>
            d.ImplementationType == typeof(TestEventHandler) ||
            d.ImplementationType == typeof(TestCommandHandler));

        Assert.That(descriptors, Is.All.Matches<ServiceDescriptor>(d =>
            d.Lifetime == ServiceLifetime.Transient));
    }

    [Test]
    public void AddAikaHandlersFromAssembly_ShouldNotRegisterInterfaces()
    {
        // Arrange
        var assembly = typeof(ITestEventHandlerInterface).Assembly;

        // Act
        _services.AddAikaHandlersFromAssembly(assembly);

        // Assert
        Assert.That(_services, Has.None.Matches<ServiceDescriptor>(d =>
            d.ImplementationType == typeof(ITestEventHandlerInterface)));
    }

    [Test]
    public void AddAikaHandlersFromAssembly_ShouldNotRegisterAbstractClasses()
    {
        // Arrange
        var assembly = typeof(AbstractTestEventHandler).Assembly;

        // Act
        _services.AddAikaHandlersFromAssembly(assembly);

        // Assert
        Assert.That(_services, Has.None.Matches<ServiceDescriptor>(d =>
            d.ImplementationType == typeof(AbstractTestEventHandler)));
    }

    [Test]
    public void AddAikaHandlersFromAssembly_ShouldHandleClassWithMultipleInterfaces()
    {
        // Arrange
        var assembly = typeof(MultiHandler).Assembly;

        // Act
        _services.AddAikaHandlersFromAssembly(assembly);
        var provider = _services.BuildServiceProvider();

        // Assert
        var eventHandler = provider.GetService<IEventHandler<MultiEvent>>();
        var commandHandler = provider.GetService<ICommandHandler<MultiCommand, TestResult>>();

        Assert.That(eventHandler, Is.InstanceOf<MultiHandler>());
        Assert.That(commandHandler, Is.InstanceOf<MultiHandler>());
    }

    [Test]
    public void AddAikaHandlersFromAssembly_ShouldReturnSameServiceCollection()
    {
        // Arrange
        var assembly = typeof(TestEventHandler).Assembly;

        // Act
        var result = _services.AddAikaHandlersFromAssembly(assembly);

        // Assert
        Assert.That(result, Is.SameAs(_services));
    }

    [Test]
    public void AddAikaHandlersFromAssembly_ShouldCreateNewInstancesForTransient()
    {
        // Arrange
        var assembly = typeof(TestEventHandler).Assembly;
        _services.AddAikaHandlersFromAssembly(assembly);
        var provider = _services.BuildServiceProvider();

        // Act
        var handler1 = provider.GetService<IEventHandler<TestEvent>>();
        var handler2 = provider.GetService<IEventHandler<TestEvent>>();

        // Assert
        Assert.That(handler1, Is.Not.SameAs(handler2));
    }

    [Test]
    public void AddAikaHandlersFromAssembly_ShouldNotRegisterStructs()
    {
        // Arrange
        var assembly = typeof(StructHandler).Assembly;

        // Act
        _services.AddAikaHandlersFromAssembly(assembly);

        // Assert
        Assert.That(_services, Has.None.Matches<ServiceDescriptor>(d =>
            d.ImplementationType == typeof(StructHandler)));
    }

    [Test]
    public void AddAikaHandlersFromAssembly_WithGenericHandler_ShouldRegister()
    {
        // Arrange
        var assembly = typeof(GenericEventHandler<>).Assembly;

        // Act & Assert
        // Generic handlers без конкретизации типа не должны регистрироваться
        Assert.DoesNotThrow(() => _services.AddAikaHandlersFromAssembly(assembly));

        // Проверяем что не зарегистрировался open generic
        Assert.That(_services, Has.None.Matches<ServiceDescriptor>(d =>
            d.ImplementationType?.IsGenericTypeDefinition == true));
    }

    [Test]
    public void AddAikaHandlersFromAssembly_WithConcreteGenericHandler_ShouldRegister()
    {
        // Arrange
        var assembly = typeof(ConcreteGenericHandler).Assembly;

        // Act
        _services.AddAikaHandlersFromAssembly(assembly);
        var provider = _services.BuildServiceProvider();

        // Assert
        var handler = provider.GetService<IEventHandler<TestEvent>>();
        Assert.That(handler, Is.InstanceOf<ConcreteGenericHandler>());
    }

    private class TestEvent : IEvent
    {
        public string EventType => throw new NotImplementedException();
    }

    private class AnotherEvent : IEvent
    {
        public string EventType => throw new NotImplementedException();
    }
    
    private class TestCommand : ICommand<TestResult>
    {
        public string CommandType => throw new NotImplementedException();
    }
    
    private class TestResult { }

    private class TestEventHandler : IEventHandler<TestEvent>
    {
        public Task HandleAsync(TestEvent @event, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    private class AnotherEventHandler : IEventHandler<AnotherEvent>
    {
        public Task HandleAsync(AnotherEvent @event, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    private class TestCommandHandler : ICommandHandler<TestCommand, TestResult>
    {
        public Task HandleAsync(TestCommand command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    private class MultiEvent : IEvent
    {
        public string EventType => throw new NotImplementedException();
    }

    private class MultiCommand : ICommand<TestResult>
    {
        public string CommandType => throw new NotImplementedException();
    }

    private class MultiHandler : IEventHandler<MultiEvent>, ICommandHandler<MultiCommand, TestResult>
    {
        public Task HandleAsync(MultiEvent @event, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task HandleAsync(MultiCommand command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    private class DoubleEventHandler : IEventHandler<TestEvent>, IEventHandler<AnotherEvent>
    {
        public Task HandleAsync(TestEvent @event, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task HandleAsync(AnotherEvent @event, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    private interface ITestEventHandlerInterface : IEventHandler<TestEvent> { }
    private abstract class AbstractTestEventHandler : IEventHandler<TestEvent>
    {
        public Task HandleAsync(TestEvent @event, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    private struct StructHandler : IEventHandler<TestEvent>
    {
        public Task HandleAsync(TestEvent @event, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    private class GenericEventHandler<T> :
        IEventHandler<T>
        where T : IEvent
    {
        public Task HandleAsync(T @event, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    private class ConcreteGenericHandler : GenericEventHandler<TestEvent> { }
    private class EmptyClass { }
}


