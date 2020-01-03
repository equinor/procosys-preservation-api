namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate
{
    public class Mode : SchemaEntityBase, IAggregateRoot
    {
        public const int TitleMinLength = 3;
        public const int TitleLengthMax = 255;

        protected Mode()
            : base(null)
        {
        }

        public Mode(string schema, string title)
            : base(schema) => Title = title;

        public string Title { get; private set; }
    }
}
