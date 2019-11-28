namespace Equinor.Procosys.Preservation.Domain
{
    public abstract class SchemaEntity : Entity
    {
        public string Schema { get; }

        // Needed for EF Core
        protected SchemaEntity()
            : base()
        {
        }

        public SchemaEntity(string schema)
        {
            Schema = schema;
        }
    }
}
