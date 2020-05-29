namespace Equinor.Procosys.Preservation.Query.GetTagFunctionDetails
{
    public class RequirementDto
    {
        public RequirementDto(int id, int requirementDefinitionId, int interValWeeks)
        {
            Id = id;
            RequirementDefinitionId = requirementDefinitionId;
            InterValWeeks = interValWeeks;
        }

        public int Id { get; }
        public int RequirementDefinitionId { get; }
        public int InterValWeeks { get; }
    }
}
