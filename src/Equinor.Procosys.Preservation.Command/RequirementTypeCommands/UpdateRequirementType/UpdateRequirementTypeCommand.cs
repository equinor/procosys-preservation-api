using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.RequirementTypeCommands.UpdateRequirementType
{
    public class UpdateRequirementTypeCommand : IRequest<Result<string>>
    {
        public UpdateRequirementTypeCommand(int requirementTypeId, string rowVersion, int sortKey, string title, string code, RequirementTypeIcon icon)
        {
            RequirementTypeId = requirementTypeId;
            RowVersion = rowVersion;
            SortKey = sortKey;
            Title = title;
            Code = code;
            Icon = icon;
        }

        public int RequirementTypeId { get; }
        public string RowVersion { get; }
        public int SortKey { get; }
        public string Code { get; }
        public string Title { get; }
        public RequirementTypeIcon Icon { get; }
    }
}
