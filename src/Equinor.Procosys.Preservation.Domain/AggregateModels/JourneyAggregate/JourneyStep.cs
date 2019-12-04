namespace Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate
{
    public class JourneyStep : SchemaEntityBase
    {
        public int JourneyModeId { get; private set; }
        public int Order { get; private set; }
    }
}
