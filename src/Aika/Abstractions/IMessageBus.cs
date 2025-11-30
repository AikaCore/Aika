namespace Aika;

public interface IMessageBus
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) 
        where TEvent: IEvent;
    
    Task<TResponse> SendAsync<TCommand, TResponse>(TCommand command, CancellationToken cancellationToken) 
        where TCommand: ICommand<TResponse>;
}
