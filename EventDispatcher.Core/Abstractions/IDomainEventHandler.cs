namespace EventDispatcher.Core.Abstractions;
internal interface IDomainEventHandler<T> where T : IDomainEvent
{
}
