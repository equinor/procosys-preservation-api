namespace Equinor.ProCoSys.Preservation.Query.GetTagFunctionDetails
{
    public class RequirementDto
    {
        public RequirementDto(int id, int requirementDefinitionId, int intervalWeeks)
        {
            Id = id;
            RequirementDefinitionId = requirementDefinitionId;
            IntervalWeeks = intervalWeeks;
        }

        public int Id { get; }
        public int RequirementDefinitionId { get; }
        public int IntervalWeeks { get; }
    }
}
