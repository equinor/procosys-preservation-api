using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public abstract class FieldValue : SchemaEntityBase
    {
        protected FieldValue()
            : base(null)
        {
        }

        protected FieldValue(string schema, Field field)
            : base(schema)
        {
            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }
            FieldId = field.Id;
        }
        
        public int FieldId { get; private set; }
    }
}
