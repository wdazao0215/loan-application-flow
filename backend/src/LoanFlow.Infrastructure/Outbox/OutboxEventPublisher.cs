using LoanFlow.Application.Abstractions;
using LoanFlow.Infrastructure.Persistence;

namespace LoanFlow.Infrastructure.Outbox;

internal sealed class OutboxEventPublisher(LoanFlowDbContext dbContext, TimeProvider timeProvider) : IEventPublisher
{
    public void Publish<TEvent>(TEvent @event)
        where TEvent : class =>
        dbContext.OutboxMessages.Add(OutboxMessage.For(@event, timeProvider.GetUtcNow()));
}
