namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate
{
    public class Mode : SchemaEntityBase, IAggregateRoot
    {
        protected Mode()
            : base(null)
        {
        }

        public Mode(string schema, string title)
            : base(schema)
        {
            Title = title;
            Schema = schema;
        }

        public string Title { get; private set; }
    }
}
