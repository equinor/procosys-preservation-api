using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.RequirementTypeCommands.CreateRequirementType
{
    public class CreateRequirementTypeCommand : IRequest<Result<int>>
    {
        public CreateRequirementTypeCommand(
            int sortKey,
            string code,
            string title,
            RequirementTypeIcon icon)
        {
            SortKey = sortKey;
            Code = code;
            Title = title;
            Icon = icon;
        }

        public int SortKey { get; }
        public string Code { get; }
        public string Title { get; }
        public RequirementTypeIcon Icon { get; }
    }
}
