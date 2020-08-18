using System;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.Procosys.Preservation.Query.GetTagsQueries.GetTags
{
    public class RequirementDto
    {
        public RequirementDto(
            int id,
            string requirementTypeCode,
            RequirementTypeIcon requirementTypeIcon,
            DateTime? nextDueTimeUtc,
            int? nextDueWeeks,
            bool readyToBePreserved)
        {
            Id = id;
            RequirementTypeCode = requirementTypeCode;
            RequirementTypeIcon = requirementTypeIcon;
            NextDueTimeUtc = nextDueTimeUtc;
            NextDueWeeks = nextDueWeeks;            
            NextDueAsYearAndWeek = NextDueTimeUtc?.FormatAsYearAndWeekString();
            ReadyToBePreserved = readyToBePreserved;
        }

        public int Id { get; }
        public string RequirementTypeCode { get; }
        public RequirementTypeIcon RequirementTypeIcon { get; }
        public DateTime? NextDueTimeUtc { get; }
        public string NextDueAsYearAndWeek { get; }

        /// <summary>
        /// NextDueWeeks shifts at Monday night regardless of where in week the NextDueTimeUtc / current time is
        /// </summary>
        public int? NextDueWeeks { get; }
        public bool ReadyToBePreserved { get; }
    }
}
