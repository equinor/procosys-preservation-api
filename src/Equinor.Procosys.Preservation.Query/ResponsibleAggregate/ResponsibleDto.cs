namespace Equinor.Procosys.Preservation.Query.ResponsibleAggregate
{
    public class ResponsibleDto
    {
        public ResponsibleDto(int id, string code, string title)
        {
            Id = id;
            Code = code;
            Title = title;
        }

        public int Id { get; }
        public string Code { get; }
        public string Title { get; }
    }
}
