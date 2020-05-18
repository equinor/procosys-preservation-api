using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.RequirementTypeCommands.UnvoidRequirementType
{
    public class UnvoidRequirementTypeCommand : IRequest<Result<string>>
    {
        public UnvoidRequirementTypeCommand(int requirementTypeId, string rowVersion)
        {
            RequirementTypeId = requirementTypeId;
            RowVersion = rowVersion;
        }

        public int RequirementTypeId { get; }
        public string RowVersion { get; }
    }
}
