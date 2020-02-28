namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate
{
    public class Responsible : SchemaEntityBase, IAggregateRoot
    {
        public const int CodeLengthMax = 255;
        public const int TitleLengthMax = 255;

        protected Responsible()
            : base(null)
        {
        }

        public Responsible(string schema, string code, string title)
            : base(schema)
        {
            Code = code;
            Title = title;
        }

        public string Code { get; private set; }
        public string Title { get; private set; }
        public bool IsVoided { get; private set; }

        public void Void() => IsVoided = true;
        public void UnVoid() => IsVoided = false;
    }
}
