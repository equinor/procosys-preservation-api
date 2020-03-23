namespace Equinor.Procosys.Preservation.Query.GetTagFunctionDetails
{
    public class RequirementDto
    {
        public RequirementDto(int id, int requirementDefinitionId)
        {
            Id = id;
            RequirementDefinitionId = requirementDefinitionId;
        }

        public int Id { get; }
        public int RequirementDefinitionId { get; }
    }
}
