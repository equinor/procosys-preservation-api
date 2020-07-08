using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.RequirementTypeCommands.UpdateRequirementType
{
    public class UpdateRequirementTypeCommand : IRequest<Result<string>>
    {
        public UpdateRequirementTypeCommand(int requirementTypeId, string rowVersion, int sortKey, string title, string code)
        {
            RequirementTypeId = requirementTypeId;
            RowVersion = rowVersion;
            SortKey = sortKey;
            Title = title;
            Code = code;
        }

        public int RequirementTypeId { get; }
        public string RowVersion { get; }
        private int SortKey { get; }
        private string Code { get; }
        private string Title { get; }
    }
}
