using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.ActionCommands.CloseAction
{
    public class CloseActionCommand : IRequest<Result<string>>, ITagCommandRequest
    {
        public CloseActionCommand(int tagId, int actionId, string rowVersion, Guid currentUserOid)
        {
            TagId = tagId;
            ActionId = actionId;
            RowVersion = rowVersion;
            CurrentUserOid = currentUserOid;
        }
        public int TagId { get; }
        public int ActionId { get; }
        public string RowVersion { get; }
        public Guid CurrentUserOid { get; }
    }
}
