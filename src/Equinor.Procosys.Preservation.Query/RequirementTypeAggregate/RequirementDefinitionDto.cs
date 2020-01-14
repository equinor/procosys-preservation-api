using System;
using System.Collections.Generic;
using System.Linq;

namespace Equinor.Procosys.Preservation.Query.RequirementTypeAggregate
{
    public class RequirementDefinitionDto
    {
        public RequirementDefinitionDto(int id, string title, bool isVoided, int defaultIntervalWeeks, int sortKey, IEnumerable<FieldDto> fields)
        {
            if (fields == null)
            {
                throw new ArgumentNullException(nameof(fields));
            }
            Id = id;
            Title = title;
            IsVoided = isVoided;
            DefaultIntervalWeeks = defaultIntervalWeeks;
            SortKey = sortKey;
            Fields = fields.OrderBy(f => f.SortKey);
        }

        public int Id { get; }
        public string Title { get; }
        public bool IsVoided { get; }
        public int DefaultIntervalWeeks { get; }
        public int SortKey { get; }
        public IEnumerable<FieldDto> Fields { get; }

        public bool NeedUserInput => Fields != null && Fields.Any(f => f.NeedUserInput);
    }
}
