namespace Equinor.Procosys.Preservation.Query.GetTagFunctionsHavingRequirement
{
    public class TagFunctionDto
    {
        public TagFunctionDto(int id, string code, string registerCode)
        {
            Id = id;
            Code = code;
            RegisterCode = registerCode;
        }

        public int Id { get; }
        public string Code { get; }
        public string RegisterCode { get; }
    }
}
