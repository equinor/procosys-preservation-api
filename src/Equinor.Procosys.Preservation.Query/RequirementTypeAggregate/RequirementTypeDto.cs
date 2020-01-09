using System.Collections.Generic;

namespace Equinor.Procosys.Preservation.Query.RequirementTypeAggregate
{
    public class RequirementTypeDto
    {
        public RequirementTypeDto(int id, string code, string title, bool isVoided, int sortKey, IEnumerable<RequirementDefinitionDto> requirementDefinitions)
        {
            Id = id;
            Code = code;
            Title = title;
            IsVoided = isVoided;
            SortKey = sortKey;
            RequirementDefinitions = requirementDefinitions;
        }

        public int Id { get; }
        public string Code { get; }
        public string Title { get; }
        public bool IsVoided { get; }
        public int SortKey { get; }
        public IEnumerable<RequirementDefinitionDto> RequirementDefinitions { get; }
    }
}
