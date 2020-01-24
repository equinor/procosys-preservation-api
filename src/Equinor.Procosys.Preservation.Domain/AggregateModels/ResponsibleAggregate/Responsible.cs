namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate
{
    public class Responsible : SchemaEntityBase, IAggregateRoot
    {
        public const int NameLengthMax = 255;

        protected Responsible()
            : base(null)
        {
        }

        public Responsible(string schema, string name)
            : base(schema) => Name = name;

        public string Name { get; private set; } // todo replace Responsible.Name with Resposible.Code
        // todo Add Responsible.Title
        public bool IsVoided { get; private set; }

        public void Void() => IsVoided = true;
        public void UnVoid() => IsVoided = false;
    }
}
