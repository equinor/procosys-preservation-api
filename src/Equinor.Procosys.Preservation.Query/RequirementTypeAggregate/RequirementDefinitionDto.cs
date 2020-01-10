using System.Collections.Generic;
using System.Linq;

namespace Equinor.Procosys.Preservation.Query.RequirementTypeAggregate
{
    public class RequirementDefinitionDto
    {
        public RequirementDefinitionDto(int id, string title, bool isVoided, int defaultInterval, int sortKey, IEnumerable<FieldDto> fields)
        {
            Id = id;
            Title = title;
            IsVoided = isVoided;
            DefaultInterval = defaultInterval;
            SortKey = sortKey;
            Fields = fields;
        }

        public int Id { get; }
        public string Title { get; }
        public bool IsVoided { get; }
        public int DefaultInterval { get; }
        public int SortKey { get; }
        public IEnumerable<FieldDto> Fields { get; }

        public bool NeedUserInput => Fields != null && Fields.Any(f => f.NeedUserInput);
    }
}
