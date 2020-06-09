using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.Preserve
{
    public class PreserveCommand : IRequest<Result<Unit>>, ITagCommandRequest
    {
        public PreserveCommand(int tagId, Guid currentUserOid)
        {
            TagId = tagId;
            CurrentUserOid = currentUserOid;
        }

        public int TagId { get; }
        public Guid CurrentUserOid { get; }
    }
}
