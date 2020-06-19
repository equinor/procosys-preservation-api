namespace Equinor.Procosys.Preservation.Query.ResponsibleAggregate
{
    public class ResponsibleDto
    {
        public ResponsibleDto(int id, string code, string description, string rowVersion)
        {
            Id = id;
            Code = code;
            Description = description;
            RowVersion = rowVersion;
        }

        public int Id { get; }
        public string Code { get; }
        public string Description { get; }
        public string RowVersion { get; }
    }
}
