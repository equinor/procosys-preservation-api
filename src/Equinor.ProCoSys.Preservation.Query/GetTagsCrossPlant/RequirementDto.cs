using System;
using Equinor.ProCoSys.Preservation.Domain;

namespace Equinor.ProCoSys.Preservation.Query.GetTagsCrossPlant
{
    public class RequirementDto
    {
        public RequirementDto(
            int id,
            string requirementTypeCode,
            string requirementDefinitionTitle,
            DateTime? nextDueTimeUtc,
            int? nextDueWeeks,
            bool readyToBePreserved)
        {
            Id = id;
            RequirementTypeCode = requirementTypeCode;
            RequirementDefinitionTitle = requirementDefinitionTitle;
            NextDueTimeUtc = nextDueTimeUtc;
            NextDueWeeks = nextDueWeeks;            
            NextDueAsYearAndWeek = NextDueTimeUtc?.FormatAsYearAndWeekString();
            ReadyToBePreserved = readyToBePreserved;
        }

        public int Id { get; }
        public string RequirementTypeCode { get; }
        public string RequirementDefinitionTitle { get; }
        public DateTime? NextDueTimeUtc { get; }
        public string NextDueAsYearAndWeek { get; }

        /// <summary>
        /// NextDueWeeks shifts at Monday night regardless of where in week the NextDueTimeUtc / current time is
        /// </summary>
        public int? NextDueWeeks { get; }
        public bool ReadyToBePreserved { get; }
    }
}
