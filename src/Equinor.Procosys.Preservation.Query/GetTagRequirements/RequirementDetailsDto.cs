using System;
using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.Procosys.Preservation.Query.GetTagRequirements
{
    public class RequirementDetailsDto
    {
        public RequirementDetailsDto(
            int id, 
            int intervalWeeks, 
            int? nextDueWeeks,
            string requirementTypeCode,
            RequirementTypeIcon requirementTypeIcon,
            string requirementTypeTitle,
            string requirementDefinitionTitle,
            DateTime? nextDueTimeUtc,
            bool readyToBePreserved,
            List<FieldDetailsDto> fields,
            string comment,
            bool isVoided,
            string rowVersion)
        {
            Id = id;
            NextDueTimeUtc = nextDueTimeUtc;
            ReadyToBePreserved = readyToBePreserved;
            Fields = fields ?? new List<FieldDetailsDto>();
            NextDueWeeks = nextDueWeeks;
            IntervalWeeks = intervalWeeks;
            RequirementTypeCode = requirementTypeCode;
            RequirementTypeIcon = requirementTypeIcon;
            RequirementTypeTitle = requirementTypeTitle;
            RequirementDefinitionTitle = requirementDefinitionTitle;
            NextDueAsYearAndWeek = NextDueTimeUtc?.FormatAsYearAndWeekString();
            Comment = comment;
            IsVoided = isVoided;
            RowVersion = rowVersion;
        }

        public int Id { get; }
        public int IntervalWeeks { get; }
        /// <summary>
        /// NextDueWeeks shifts at Monday night regardless of where in week the NextDueTimeUtc / current time is
        /// </summary>
        public int? NextDueWeeks { get; }
        public string RequirementTypeCode { get; }
        public RequirementTypeIcon RequirementTypeIcon { get; }
        public string RequirementTypeTitle { get; }
        public string RequirementDefinitionTitle { get; }
        public DateTime? NextDueTimeUtc { get; }
        public string NextDueAsYearAndWeek { get; }
        public bool ReadyToBePreserved { get; }
        public List<FieldDetailsDto> Fields { get; }
        public string Comment { get; }
        public bool IsVoided { get;  }
        public string RowVersion { get; }
    }
}
