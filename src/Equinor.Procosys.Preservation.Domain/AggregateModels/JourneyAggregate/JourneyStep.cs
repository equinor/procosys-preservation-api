namespace Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate
{
    public class JourneyStep : SchemaEntity
    {
        public int JourneyModeId { get; private set; }
        public int Order { get; private set; }
    }
}
