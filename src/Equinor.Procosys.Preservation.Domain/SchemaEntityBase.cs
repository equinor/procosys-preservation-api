namespace Equinor.Procosys.Preservation.Domain
{
    public abstract class SchemaEntityBase : EntityBase
    {
        protected SchemaEntityBase(string schema)
        {
            Schema = schema;
        }

        public string Schema { get; protected set; }
    }
}
