using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.RequirementTypeCommands.VoidRequirementDefinition
{
    public class VoidRequirementDefinitionCommand : IRequest<Result<string>>
    {
        public VoidRequirementDefinitionCommand(int requirementTypeId, int requirementDefinitionId, string rowVersion)
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
