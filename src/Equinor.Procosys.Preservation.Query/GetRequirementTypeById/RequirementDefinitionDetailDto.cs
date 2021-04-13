using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Query.GetRequirementTypeById
{
    public class RequirementDefinitionDetailDto
    {
        public RequirementDefinitionDetailDto(
            int id,
            string title,
            bool isInUse,
            bool isVoided,
            int defaultIntervalWeeks,
            RequirementUsage usage,
            int sortKey,
            bool needsUserInput,
            IEnumerable<FieldDetailsDto> fields,
            string rowVersion)
        {
            if (fields == null)
            {
                throw new ArgumentNullException(nameof(fields));
            }
            Id = id;
            Title = title;
            IsInUse = isInUse;
            IsVoided = isVoided;
            DefaultIntervalWeeks = defaultIntervalWeeks;
            Usage = usage;
            SortKey = sortKey;
            NeedsUserInput = needsUserInput;
            Fields = fields.OrderBy(f => f.SortKey);
            RowVersion = rowVersion;
        }

        public int Id { get; }
        public string Title { get; }
        public bool IsInUse { get; }
        public bool IsVoided { get; }
        public int DefaultIntervalWeeks { get; }
        public RequirementUsage Usage { get; }
        public int SortKey { get; }
        public bool NeedsUserInput { get; }
        public IEnumerable<FieldDetailsDto> Fields { get; }
        public string RowVersion { get; }
    }
}
