using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.RequirementTypeCommands.AddRequirementType
{
    public class AddRequirementTypeCommand : IRequest<Result<int>>
    {
        public AddRequirementTypeCommand(int sortKey, string code, string title)
        {
            SortKey = sortKey;
            Code = code;
            Title = title;
        }

        public int SortKey { get; }
        public string Code { get; }
        public string Title { get; }
    }
}
