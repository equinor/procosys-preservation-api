using System;
using Equinor.Procosys.Preservation.Domain;

namespace Equinor.Procosys.Preservation.Query.ProjectAggregate
{
    public class RequirementDto
    {
        public RequirementDto(int id, int requirementDefinitionId, DateTime? nextDueTimeUtc, TimeSpan timeUntilNextDueTime)
        {
            Id = id;
            RequirementDefinitionId = requirementDefinitionId;
            NextDueTimeUtc = nextDueTimeUtc;
            NextDueWeeks = timeUntilNextDueTime.Weeks();
        }

        public int Id { get; }
        public int RequirementDefinitionId { get; }
        public DateTime? NextDueTimeUtc { get; }

        public string NextDueAsYearAndWeek => NextDueTimeUtc?.FormatAsYearAndWeekString();

        public int NextDueWeeks { get; }
    }
}
