namespace Equinor.Procosys.Preservation.Query.GetUniqueTagAreas
{
    public class AreaCodeDto
    {
        public AreaCodeDto(string code, string description)
        {
            Code = code;
            Description = description;
        }

        public string Code { get;}
        public string Description { get; }
    }
}
