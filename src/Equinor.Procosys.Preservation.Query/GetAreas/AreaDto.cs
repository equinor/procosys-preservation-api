namespace Equinor.Procosys.Preservation.Query.GetAreas
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
