using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.RequirementTypeCommands.VoidRequirementType
{
    public class VoidRequirementTypeCommand : IRequest<Result<string>>
    {
        public VoidRequirementTypeCommand(int requirementTypeId, string rowVersion)
        {
            RequirementTypeId = requirementTypeId;
            RowVersion = rowVersion;
        }

        public int RequirementTypeId { get; }
        public string RowVersion { get; }
    }
}
