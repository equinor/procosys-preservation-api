namespace Equinor.Procosys.Preservation.Query.TagAggregate
{
    public class RequirementDto
    {
        public RequirementDto(int id, int requirementDefinitionId, bool isVoided, int intervalWeeks)
        {
            Id = id;
            RequirementDefinitionId = requirementDefinitionId;
            IsVoided = isVoided;
            IntervalWeeks = intervalWeeks;
        }

        public int Id { get; set; }
        public int RequirementDefinitionId { get; }
        public bool IsVoided { get; }
        public int IntervalWeeks { get; }
    }
}
