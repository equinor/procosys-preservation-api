using System;
using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain;

namespace Equinor.Procosys.Preservation.Query.GetTagRequirements
{
    public class RequirementDto
    {
        public RequirementDto(
            int id, 
            int intervalWeeks, 
            int? nextDueWeeks,
            string requirementTypeCode,
            string requirementTypeTitle,
            string requirementDefinitionTitle,
            DateTime? nextDueTimeUtc,
            bool readyToBePreserved,
            List<FieldDto> fields,
            string comment)
        {
            Id = id;
            NextDueTimeUtc = nextDueTimeUtc;
            ReadyToBePreserved = readyToBePreserved;
            Fields = fields ?? new List<FieldDto>();
            NextDueWeeks = nextDueWeeks;
            IntervalWeeks = intervalWeeks;
            RequirementTypeCode = requirementTypeCode;
            RequirementTypeTitle = requirementTypeTitle;
            RequirementDefinitionTitle = requirementDefinitionTitle;
            NextDueAsYearAndWeek = NextDueTimeUtc?.FormatAsYearAndWeekString();
            Comment = comment;
        }

        public int Id { get; }
        public int IntervalWeeks { get; }
        /// <summary>
        /// NextDueWeeks shifts at Monday night regardless of where in week the NextDueTimeUtc / current time is
        /// </summary>
        public int? NextDueWeeks { get; }
        public string RequirementTypeCode { get; }
        public string RequirementTypeTitle { get; }
        public string RequirementDefinitionTitle { get; }
        public DateTime? NextDueTimeUtc { get; }
        public string NextDueAsYearAndWeek { get; }
        public bool ReadyToBePreserved { get; }
        public List<FieldDto> Fields { get; }
        public string Comment { get; }
    }
}
