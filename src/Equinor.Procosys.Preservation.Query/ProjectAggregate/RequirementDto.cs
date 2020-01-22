using System;
using Equinor.Procosys.Preservation.Domain;

namespace Equinor.Procosys.Preservation.Query.ProjectAggregate
{
    public class RequirementDto
    {
        public RequirementDto(DateTime? nextDueTimeUtc) => NextDueTimeUtc = nextDueTimeUtc;

        public DateTime? NextDueTimeUtc { get; }

        public string NextDueAsYearAndWeek => NextDueTimeUtc?.FormatAsYearAndWeekString();
    }
}
