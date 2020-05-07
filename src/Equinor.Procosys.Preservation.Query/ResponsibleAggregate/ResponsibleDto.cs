namespace Equinor.Procosys.Preservation.Query.ResponsibleAggregate
{
    public class ResponsibleDto
    {
        public ResponsibleDto(int id, string code, string title, string rowVersion)
        {
            Id = id;
            Code = code;
            Title = title;
            RowVersion = rowVersion;
        }

        public int Id { get; }
        public string Code { get; }
        public string Title { get; }
        public string RowVersion { get; }
    }
}
