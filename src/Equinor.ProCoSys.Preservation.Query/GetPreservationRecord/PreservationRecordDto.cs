using System.Collections.Generic;

namespace Equinor.ProCoSys.Preservation.Query.GetPreservationRecord
{
    public class PreservationRecordDto
    {
        public PreservationRecordDto(
            int id,
            bool bulkPreserved,
            RequirementTypeDetailsDto requirementType,
            RequirementDefinitionDetailDto requirementDefinition,
            int intervalWeeks,
            string comment,
            List<FieldDetailsDto> fields)
        {
            Id = id;
            BulkPreserved = bulkPreserved;
            RequirementType = requirementType;
            RequirementDefinition = requirementDefinition;
            IntervalWeeks = intervalWeeks;
            Comment = comment;
            Fields = fields;
        }

        public int Id { get; }
        public bool BulkPreserved { get; }
        public RequirementTypeDetailsDto RequirementType { get; }
        public RequirementDefinitionDetailDto RequirementDefinition { get; }
        public int IntervalWeeks { get; }
        public string Comment { get; }
        public List<FieldDetailsDto> Fields { get; }
    }
}
