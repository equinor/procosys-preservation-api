namespace Equinor.Procosys.Preservation.Query.TagAggregate
{
    public class RequirementDto
    {
        public RequirementDto(int id, int requirementDefinitionId, bool isVoided, int interval)
        {
            Id = id;
            RequirementDefinitionId = requirementDefinitionId;
            IsVoided = isVoided;
            Interval = interval;
        }

        public int Id { get; set; }
        public int RequirementDefinitionId { get; }
        public bool IsVoided { get; }
        public int Interval { get; }
    }
}
