using System.Collections.Generic;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate
{
    public class Journey : SchemaEntityBase, IAggregateRoot
    {
        private List<JourneyStep> _steps = new List<JourneyStep>();
        
        public Journey()
        {
        }

        public IReadOnlyCollection<JourneyStep> Steps => _steps.AsReadOnly();
    }
}
