using System;
using System.Collections.Generic;
using System.Linq;

namespace Equinor.Procosys.Preservation.Query.RequirementTypeAggregate
{
    public class RequirementTypeDto
    {
        public RequirementTypeDto(
            int id,
            string code,
            string title,
            bool isVoided,
            int sortKey,
            IEnumerable<RequirementDefinitionDto> requirementDefinitions,
            ulong rowVersion)
        {
            if (requirementDefinitions == null)
            {
                throw new ArgumentNullException(nameof(requirementDefinitions));
            }
            Id = id;
            Code = code;
            Title = title;
            IsVoided = isVoided;
            SortKey = sortKey;
            RowVersion = rowVersion;
            RequirementDefinitions = requirementDefinitions.OrderBy(rd => rd.NeedsUserInput).ThenBy(rd => rd.SortKey);
        }

        public int Id { get; }
        public string Code { get; }
        public string Title { get; }
        public bool IsVoided { get; }
        public int SortKey { get; }
        public ulong RowVersion { get; }
        public IEnumerable<RequirementDefinitionDto> RequirementDefinitions { get; }
    }
}
