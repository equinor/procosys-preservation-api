namespace Equinor.Procosys.Preservation.Domain
{
    /// <summary>
    /// Base class for entities to be filtered by schema
    /// </summary>
    public abstract class SchemaEntityBase : EntityBase
    {
        public const int SchemaLengthMax = 255;

        protected SchemaEntityBase(string schema) => Schema = schema;

        public virtual string Schema { get; private set; }
    }
}
