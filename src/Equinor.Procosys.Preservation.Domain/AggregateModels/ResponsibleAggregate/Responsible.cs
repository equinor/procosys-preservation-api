namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate
{
    public class Responsible : Entity, IAggregateRoot
    {
        public string Name { get; set; }
    }
}
