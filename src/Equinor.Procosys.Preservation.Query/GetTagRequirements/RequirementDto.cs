using System;
using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain;

namespace Equinor.Procosys.Preservation.Query.GetTagRequirements
{
    public class RequirementDto
    {
        public RequirementDto(
            int id,
            string requirementTypeCode,
            string requirementTypeTitle,
            string requirementDefinitionTitle,
            DateTime? nextDueTimeUtc,
            bool readyToBePreserved,
            List<FieldDto> fields)
        {
            Id = id;
            NextDueTimeUtc = nextDueTimeUtc;
            ReadyToBePreserved = readyToBePreserved;
            Fields = fields;
            RequirementTypeCode = requirementTypeCode;
            RequirementTypeTitle = requirementTypeTitle;
            RequirementDefinitionTitle = requirementDefinitionTitle;
            NextDueAsYearAndWeek = NextDueTimeUtc?.FormatAsYearAndWeekString();
        }

        public int Id { get; }
        public string RequirementTypeCode { get; }
        public string RequirementTypeTitle { get; }
        public string RequirementDefinitionTitle { get; }
        public DateTime? NextDueTimeUtc { get; }
        public string NextDueAsYearAndWeek { get; }
        public bool ReadyToBePreserved { get; }
        public List<FieldDto> Fields { get; }
    }
}
