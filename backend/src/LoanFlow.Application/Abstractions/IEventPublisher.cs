namespace LoanFlow.Application.Abstractions;

public interface IEventPublisher
{
    void Publish<TEvent>(TEvent @event)
        where TEvent : class;
}
