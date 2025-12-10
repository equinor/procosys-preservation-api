namespace Equinor.ProCoSys.Preservation.Query.GetUniqueTagAreas
{
    public class AreaDto
    {
        public AreaDto(string code, string description)
        {
            Code = code;
            Description = description;
        }

        public string Code { get; }
        public string Description { get; }
    }
}
