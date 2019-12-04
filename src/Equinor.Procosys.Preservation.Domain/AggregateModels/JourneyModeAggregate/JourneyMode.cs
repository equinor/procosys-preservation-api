namespace Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyModeAggregate
{
    public class JourneyMode : EntityBase, IAggregateRoot
    {
        protected JourneyMode()
        {
        }

        public JourneyMode(string name, string schema)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}
