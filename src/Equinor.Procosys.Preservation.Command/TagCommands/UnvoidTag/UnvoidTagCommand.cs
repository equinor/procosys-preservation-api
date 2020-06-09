using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.UnvoidTag
{
    public class UnvoidTagCommand : IRequest<Result<string>>, ITagCommandRequest
    {
        public UnvoidTagCommand(int tagId, string rowVersion, Guid currentUserOid)
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
