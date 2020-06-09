using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.RequirementCommands.Preserve
{
    public class PreserveCommand : IRequest<Result<Unit>>, ITagCommandRequest
    {
        public PreserveCommand(int tagId, int requirementId, Guid currentUserOid)
        {
            TagId = tagId;
            RequirementId = requirementId;
            CurrentUserOid = currentUserOid;
        }

        public int TagId { get; }
        public int RequirementId { get; }
        public Guid CurrentUserOid { get; }
    }
}
