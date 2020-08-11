using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.Procosys.Preservation.Query.GetPreservationRecord
{
    public class PreservationRecordDto
    {
        public PreservationRecordDto(
            int id,
            bool bulkPreserved,
            string requirementTypeTitle,
            string requirementTypeCode,
            RequirementTypeIcon requirementTypeIcon,
            string requirementDefinitionTitle,
            int intervalWeeks,
            string comment,
            List<FieldDto> fields)
        {
            Id = id;
            BulkPreserved = bulkPreserved;
            RequirementTypeTitle = requirementTypeTitle;
            RequirementTypeCode = requirementTypeCode;
            RequirementTypeIcon = requirementTypeIcon;
            RequirementDefinitionTitle = requirementDefinitionTitle;
            IntervalWeeks = intervalWeeks;
            Comment = comment;
            Fields = fields;
        }

        public int Id { get; }
        public bool BulkPreserved { get; }
        public string RequirementTypeTitle { get; }
        public string RequirementTypeCode { get; }
        public RequirementTypeIcon RequirementTypeIcon { get; }
        public string RequirementDefinitionTitle { get; }
        public int IntervalWeeks { get; }
        public string Comment { get; }
        public List<FieldDto> Fields { get; }
    }
}
