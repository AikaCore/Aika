namespace Aika;

public interface ICommandHandler<TCommand, TResponse>
    where TCommand: ICommand<TResponse>
{
    Task HandleAsync(TCommand command, CancellationToken cancellationToken);
}
