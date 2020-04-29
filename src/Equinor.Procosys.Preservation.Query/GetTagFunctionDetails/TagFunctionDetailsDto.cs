using System.Collections.Generic;

namespace Equinor.Procosys.Preservation.Query.GetTagFunctionDetails
{
    public class TagFunctionDetailsDto
    {
        public TagFunctionDetailsDto(
            int id,
            string code,
            string description,
            string registerCode,
            bool isVoided,
            IEnumerable<RequirementDto> requirements,
            ulong rowVersion)
        {
            Id = id;
            Code = code;
            Description = description;
            RegisterCode = registerCode;
            IsVoided = isVoided;
            Requirements = requirements;
            RowVersion = rowVersion;
        }

        public int Id { get; }
        public string Code { get; }
        public string Description { get; }
        public string RegisterCode { get; }
        public bool IsVoided { get; }
        public IEnumerable<RequirementDto> Requirements { get; }
        public ulong RowVersion { get; }
    }
}
