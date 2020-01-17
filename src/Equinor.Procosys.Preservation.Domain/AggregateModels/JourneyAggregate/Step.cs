using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate
{
    public class Step : SchemaEntityBase
    {
        protected Step()
            : base(null)
        {
        }

        public Step(string schema, Mode mode, Responsible responsible)
            : base(schema)
        {
            if (mode == null)
            {
                throw new ArgumentNullException(nameof(mode));
            }

            if (responsible == null)
            {
                throw new ArgumentNullException(nameof(responsible));
            }

            ModeId = mode.Id;
            ResponsibleId = responsible.Id;
        }

        public int ModeId { get; private set; }
        public int ResponsibleId { get; private set; }
        public bool IsVoided { get; private set; }

        public void Void() => IsVoided = true;
        public void UnVoid() => IsVoided = false;
    }
}
