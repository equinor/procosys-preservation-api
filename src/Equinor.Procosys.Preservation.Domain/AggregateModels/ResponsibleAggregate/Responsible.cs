namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate
{
    public class Responsible : EntityBase, IAggregateRoot
    {
        public string Name { get; set; }
    }
}
