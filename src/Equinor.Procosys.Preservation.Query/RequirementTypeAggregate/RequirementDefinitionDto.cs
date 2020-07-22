using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.Procosys.Preservation.Query.RequirementTypeAggregate
{
    public class RequirementDefinitionDto
    {
        public RequirementDefinitionDto(
            int id,
            string title,
            bool isVoided,
            int defaultIntervalWeeks,
            RequirementUsage usage,
            int sortKey,
            bool needsUserInput,
            string rowVersion,
            IEnumerable<FieldDto> fields)
        {
            if (fields == null)
            {
                throw new ArgumentNullException(nameof(fields));
            }
            Id = id;
            Title = title;
            IsVoided = isVoided;
            DefaultIntervalWeeks = defaultIntervalWeeks;
            Usage = usage;
            SortKey = sortKey;
            Fields = fields.OrderBy(f => f.SortKey);
            NeedsUserInput = needsUserInput;
            RowVersion = rowVersion;
        }

        public int Id { get; }
        public string Title { get; }
        public bool IsVoided { get; }
        public int DefaultIntervalWeeks { get; }
        public RequirementUsage Usage { get; }
        public int SortKey { get; }
        public IEnumerable<FieldDto> Fields { get; }
        public bool NeedsUserInput { get; }
        public string RowVersion { get; }
    }
}
