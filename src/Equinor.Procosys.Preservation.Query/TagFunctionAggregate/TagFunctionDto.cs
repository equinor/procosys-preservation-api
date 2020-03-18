using System.Collections.Generic;

namespace Equinor.Procosys.Preservation.Query.TagFunctionAggregate
{
    public class TagFunctionDto
    {
        public TagFunctionDto(int id, string code, string description, string registerCode, IEnumerable<RequirementDto> requirements)
        {
            Id = id;
            Code = code;
            Description = description;
            RegisterCode = registerCode;
            Requirements = requirements;
        }

        public int Id { get; }
        public string Code { get; }
        public string Description { get; }
        public string RegisterCode { get; }
        public IEnumerable<RequirementDto> Requirements { get; }
    }
}
