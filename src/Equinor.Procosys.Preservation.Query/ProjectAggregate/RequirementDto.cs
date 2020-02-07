using System;
using Equinor.Procosys.Preservation.Domain;

namespace Equinor.Procosys.Preservation.Query.ProjectAggregate
{
    public class RequirementDto
    {
        public RequirementDto(
            int id,
            int requirementDefinitionId,
            DateTime? nextDueTimeUtc,
            int? nextDueWeeks,
            bool readyToBePreserved,
            bool readyToBeBulkPreserved
            )
        {
            Id = id;
            RequirementDefinitionId = requirementDefinitionId;
            NextDueTimeUtc = nextDueTimeUtc;
            NextDueWeeks = nextDueWeeks;            
            NextDueAsYearAndWeek = NextDueTimeUtc?.FormatAsYearAndWeekString();
            ReadyToBePreserved = readyToBePreserved;
            ReadyToBeBulkPreserved = readyToBeBulkPreserved;
        }

        public int Id { get; }
        public int RequirementDefinitionId { get; }
        public DateTime? NextDueTimeUtc { get; }
        public string NextDueAsYearAndWeek { get; }

        /// <summary>
        /// NextDueWeeks shifts at Monday night regardless of where in week the NextDueTimeUtc / current time is
        /// </summary>
        public int? NextDueWeeks { get; }
        public bool ReadyToBePreserved { get; }
        public bool ReadyToBeBulkPreserved { get; }
    }
}
