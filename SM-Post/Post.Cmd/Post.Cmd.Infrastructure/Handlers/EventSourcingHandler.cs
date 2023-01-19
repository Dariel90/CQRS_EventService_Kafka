using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Infrastructure.Handlers
{
    public class EventSourcingHandler : IEventSourcingHandler<PostAggregate>
    {
        private readonly IEventStore eventStore;
        private readonly IEventProducer eventProducer;

        public EventSourcingHandler(IEventStore eventStore, IEventProducer eventProducer)
        {
            this.eventStore = eventStore;
            this.eventProducer = eventProducer;
        }

        public async Task<PostAggregate> GetIdAsync(Guid aggregateId)
        {
            var aggregate = new PostAggregate();
            var events = await eventStore.GetEventsAsync(aggregateId);

            if (events == null || !events.Any())
            {
                return aggregate;
            }

            aggregate.ReplayEvents(events);
            aggregate.Version = events.Select(x => x.Version).Max();

            return aggregate;
        }

        public async Task SaveAsync(AggregateRoot aggregate)
        {
            await eventStore.SaveEventAsync(aggregate.Id, aggregate.GetUncommitedChanges(), aggregate.Version);
            aggregate.MarkChangesAsCommited();
        }

        public async Task RepublishEventsAsync()
        {
            var aggregateIds = await this.eventStore.GetAggregateIdAsync();
            if (aggregateIds == null || !aggregateIds.Any())
                return;
            var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
            foreach (var aggregateId in aggregateIds)
            {
                var aggregate = await GetIdAsync(aggregateId);
                if (aggregate == null || !aggregate.Active)
                    continue;

                var events = await this.eventStore.GetEventsAsync(aggregateId);
                foreach (var @event in events)
                {
                    await this.eventProducer.ProduceAsync(topic, @event);
                }
            }
        }
    }
}