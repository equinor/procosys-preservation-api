using System;
using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain;

namespace Equinor.Procosys.Preservation.Query.GetTagRequirements
{
    public class RequirementDetailsDto
    {
        public RequirementDetailsDto(
            int id, 
            int intervalWeeks, 
            int? nextDueWeeks,
            RequirementTypeDetailsDto requirementType,
            RequirementDefinitionDetailDto requirementDefinition,
            DateTime? nextDueTimeUtc,
            bool readyToBePreserved,
            List<FieldDetailsDto> fields,
            string comment,
            bool isVoided,
            bool isInUse,
            string rowVersion)
        {
            Id = id;
            NextDueTimeUtc = nextDueTimeUtc;
            ReadyToBePreserved = readyToBePreserved;
            Fields = fields ?? new List<FieldDetailsDto>();
            NextDueWeeks = nextDueWeeks;
            IntervalWeeks = intervalWeeks;
            RequirementType = requirementType;
            RequirementDefinition = requirementDefinition;
            NextDueAsYearAndWeek = NextDueTimeUtc?.FormatAsYearAndWeekString();
            Comment = comment;
            IsVoided = isVoided;
            IsInUse = isInUse;
            RowVersion = rowVersion;
        }

        public int Id { get; }
        public int IntervalWeeks { get; }
        /// <summary>
        /// NextDueWeeks shifts at Monday night regardless of where in week the NextDueTimeUtc / current time is
        /// </summary>
        public int? NextDueWeeks { get; }
        public RequirementTypeDetailsDto RequirementType { get; }
        public RequirementDefinitionDetailDto RequirementDefinition { get; }
        public DateTime? NextDueTimeUtc { get; }
        public string NextDueAsYearAndWeek { get; }
        public bool ReadyToBePreserved { get; }
        public List<FieldDetailsDto> Fields { get; }
        public string Comment { get; }
        public bool IsVoided { get;  }
        public bool IsInUse { get; set; }
        public string RowVersion { get; }
    }
}
