using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.ModeCommands.DeleteMode
{
    public class DeleteModeCommand : IRequest<Result<Unit>>
    {
        public DeleteModeCommand(int modeId, string rowVersion, Guid currentUserOid)
        {
            ModeId = modeId;
            RowVersion = rowVersion;
            CurrentUserOid = currentUserOid;
        }

        public int ModeId { get; }
        public string RowVersion { get; }
        public Guid CurrentUserOid { get; }
    }
}
