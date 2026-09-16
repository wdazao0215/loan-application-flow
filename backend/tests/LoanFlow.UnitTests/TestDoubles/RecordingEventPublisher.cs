using LoanFlow.Application.Abstractions;

namespace LoanFlow.UnitTests.TestDoubles;

internal sealed class RecordingEventPublisher : IEventPublisher
{
    public List<object> PublishedEvents { get; } = [];

    public void Publish<TEvent>(TEvent @event)
        where TEvent : class => PublishedEvents.Add(@event);
}
