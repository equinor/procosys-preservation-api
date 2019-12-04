using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate
{
    public class Step : SchemaEntityBase
    {
        private Step()
            : base(null)
        {
        }

        public Step(string schema, Mode mode, Responsible responsible)
            : base(schema)
        {
            if (mode == null)
                throw new ArgumentNullException($"{nameof(mode)} cannot be null");
            if (responsible == null)
                throw new ArgumentNullException($"{nameof(responsible)} cannot be null");

            ModeId = mode.Id;
            ResponsibleId = responsible.Id;
        }

        public int ModeId { get; private set; }
        public int ResponsibleId { get; private set; }
    }
}
