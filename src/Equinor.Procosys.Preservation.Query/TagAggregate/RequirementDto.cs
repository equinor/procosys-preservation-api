using System;
using Equinor.Procosys.Preservation.Domain;

namespace Equinor.Procosys.Preservation.Query.TagAggregate
{
    public class RequirementDto
    {
        public RequirementDto(bool needsUserInput, DateTime? nextDueTimeUtc)
        {
            NeedsUserInput = needsUserInput;
            NextDueTimeUtc = nextDueTimeUtc;
        }

        public DateTime? NextDueTimeUtc { get; }

        public bool NeedsUserInput { get; }

        public string NextDueAsYearAndWeek => NextDueTimeUtc?.FormatAsYearAndWeekString();
    }
}
