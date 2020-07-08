using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.RequirementTypeCommands.DeleteRequirementType
{
    public class DeleteRequirementTypeCommand : IRequest<Result<Unit>>
    {
        public DeleteRequirementTypeCommand(int requirementTypeId, string rowVersion)
        {
            RequirementTypeId = requirementTypeId;
            RowVersion = rowVersion;
        }

        public int RequirementTypeId { get; }
        public string RowVersion { get; }
    }
}
