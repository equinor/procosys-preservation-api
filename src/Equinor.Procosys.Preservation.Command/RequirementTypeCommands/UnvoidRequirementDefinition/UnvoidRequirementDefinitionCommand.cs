using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.RequirementTypeCommands.UnvoidRequirementDefinition
{
    public class UnvoidRequirementDefinitionCommand : IRequest<Result<string>>
    {
        public UnvoidRequirementDefinitionCommand(int requirementTypeId, int requirementDefinitionId, string rowVersion)
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
