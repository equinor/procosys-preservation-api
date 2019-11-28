namespace Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyModeAggregate
{
    public class JourneyMode : SchemaEntity, IAggregateRoot
    {
        protected JourneyMode()
        {
        }

        public JourneyMode(string name, string schema)
            : base(schema)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}
