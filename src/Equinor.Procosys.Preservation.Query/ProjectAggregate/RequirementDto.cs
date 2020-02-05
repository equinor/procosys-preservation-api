using System;
using Equinor.Procosys.Preservation.Domain;

namespace Equinor.Procosys.Preservation.Query.ProjectAggregate
{
    public class RequirementDto
    {
        public RequirementDto(
            int id,
            int requirementDefinitionId,
            DateTime currentTimeUtc,
            DateTime? nextDueTimeUtc,
            bool readyToBePreserved)
        {
            Id = id;
            RequirementDefinitionId = requirementDefinitionId;
            NextDueTimeUtc = nextDueTimeUtc;
            
            if (nextDueTimeUtc.HasValue)
            {
                NextDueWeeks = currentTimeUtc.GetWeeksUntil(nextDueTimeUtc.Value);
            }
            NextDueAsYearAndWeek = NextDueTimeUtc?.FormatAsYearAndWeekString();
            ReadyToBePreserved = readyToBePreserved;
        }

        public int Id { get; }
        public int RequirementDefinitionId { get; }
        public DateTime? NextDueTimeUtc { get; }
        public string NextDueAsYearAndWeek { get; }

        /// <summary>
        /// NextDueWeeks shifts at Monday night regardless of where in week the NextDueTimeUtc / current time is
        /// </summary>
        public int NextDueWeeks { get; }
        public bool ReadyToBePreserved { get; }
    }
}
