using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.RequirementTypeCommands.DeleteRequirementDefinition
{
    public class DeleteRequirementDefinitionCommand : IRequest<Result<Unit>>
    {
        public DeleteRequirementDefinitionCommand(int requirementTypeId, int requirementDefinitionId, string rowVersion)
        {
            RequirementTypeId = requirementTypeId;
            RequirementDefinitionId = requirementDefinitionId;
            RowVersion = rowVersion;
        }

        public int RequirementTypeId { get; }
        public int RequirementDefinitionId { get; }
        public string RowVersion { get; }
    }
}
