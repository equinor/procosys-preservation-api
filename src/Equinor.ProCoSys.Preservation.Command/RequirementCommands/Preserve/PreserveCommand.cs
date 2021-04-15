using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.RequirementCommands.Preserve
{
    public class PreserveCommand : IRequest<Result<Unit>>, ITagCommandRequest
    {
        public PreserveCommand(int tagId, int requirementId)
        {
            TagId = tagId;
            RequirementId = requirementId;
        }

        public int TagId { get; }
        public int RequirementId { get; }
    }
}
