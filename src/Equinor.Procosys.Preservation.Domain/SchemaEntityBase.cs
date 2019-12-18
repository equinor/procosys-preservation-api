using System;

namespace Equinor.Procosys.Preservation.Domain
{
    /// <summary>
    /// Base class for entities to be filtered by schema
    /// </summary>
    public abstract class SchemaEntityBase : EntityBase
    {
        protected SchemaEntityBase(string schema)
        {
            if (string.IsNullOrEmpty(schema))
            {
                throw new ArgumentNullException(nameof(schema));
            }

            Schema = schema;
        }

        public virtual string Schema { get; private set; }
    }
}
