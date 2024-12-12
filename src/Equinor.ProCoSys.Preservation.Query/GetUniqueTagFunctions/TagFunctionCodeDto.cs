namespace Equinor.ProCoSys.Preservation.Query.GetUniqueTagFunctions
{
    public class TagFunctionCodeDto
    {
        
        public TagFunctionCodeDto(string code, string description)
        {
            Code = code;
            Description = description;
        }

        public string Code { get; }
        public string Description { get; set; }
    }
}
