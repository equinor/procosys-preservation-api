namespace Equinor.Procosys.Preservation.Query.ResponsibleAggregate
{
    public class ResponsibleDto
    {
        public ResponsibleDto(int id, string code)
        {
            Id = id;
            Code = code;
        }

        public int Id { get; }
        public string Code { get; }
    }
}
