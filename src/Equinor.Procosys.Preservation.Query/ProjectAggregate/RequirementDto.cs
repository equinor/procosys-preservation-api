using System;
using Equinor.Procosys.Preservation.Domain;

namespace Equinor.Procosys.Preservation.Query.ProjectAggregate
{
    public class RequirementDto
    {
        public RequirementDto(int id, int requirementDefinitionId, DateTime currentTimeUtc, DateTime? nextDueTimeUtc)
        {
            Id = id;
            RequirementDefinitionId = requirementDefinitionId;
            NextDueTimeUtc = nextDueTimeUtc;
            
            if (nextDueTimeUtc.HasValue)
            {
                NextDueWeeks = nextDueTimeUtc.Value.GetWeeksReferencedFromStartOfWeek(currentTimeUtc);
            }
            NextDueAsYearAndWeek = NextDueTimeUtc?.FormatAsYearAndWeekString();
        }

        public int Id { get; }
        public int RequirementDefinitionId { get; }
        public DateTime? NextDueTimeUtc { get; }
        public string NextDueAsYearAndWeek { get; }

        /// <summary>
        /// NextDueWeeks shifts at Monday night regardless of where in week the NextDueTimeUtc / current time is
        /// </summary>
        public int NextDueWeeks { get; }
    }
}
