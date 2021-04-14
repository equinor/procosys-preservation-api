namespace Equinor.ProCoSys.Preservation.Query.GetTagDetails
{
    public class ResponsibleDetailsDto
    {
        public ResponsibleDetailsDto(int id, string code, string description)
        {
            Id = id;
            Code = code;
            Description = description;
        }

        public int Id { get; }
        public string Code { get; }
        public string Description { get; }
    }
}
