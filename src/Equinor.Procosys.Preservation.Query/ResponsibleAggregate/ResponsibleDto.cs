namespace Equinor.Procosys.Preservation.Query.ResponsibleAggregate
{
    public class ResponsibleDto
    {
        public ResponsibleDto(int id, string name)        {
            Id = id;
            Name = name;
        }

        public int Id { get; }
        public string Name { get; }
    }
}
