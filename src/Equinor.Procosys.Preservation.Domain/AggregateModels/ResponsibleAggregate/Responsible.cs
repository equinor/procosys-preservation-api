namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate
{
    public class Responsible : SchemaEntityBase, IAggregateRoot
    {
        protected Responsible()
            : base(null)
        {
        }

        public Responsible(string schema, string name)
            : base(schema)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}
