using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.VoidTag
{
    public class VoidTagCommand : IRequest<Result<string>>, ITagCommandRequest
    {
        public VoidTagCommand(int tagId, string rowVersion, Guid currentUserOid)
        {
            TagId = tagId;
            RowVersion = rowVersion;
            CurrentUserOid = currentUserOid;
        }

        public int TagId { get; }
        public string RowVersion { get; }
        public Guid CurrentUserOid { get; }
    }
}
