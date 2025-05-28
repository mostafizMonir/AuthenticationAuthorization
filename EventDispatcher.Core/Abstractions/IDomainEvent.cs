namespace EventDispatcher.Core.Abstractions;

    public interface IDomainEvent
    {
        DateTime OccurredOn { get; set; }
    }

