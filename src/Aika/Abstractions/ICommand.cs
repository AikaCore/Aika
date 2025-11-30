namespace Aika;

public interface ICommand<TResponse>
{
    string CommandType { get; }
}
