using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.ModeCommands.UpdateMode
{
    public class UpdateModeCommand : IRequest<Result<string>>
    {
        public UpdateModeCommand(int modeId, string title, bool forSupplier, string rowVersion, Guid currentUserOid)
        {
            ModeId = modeId;
            Title = title;
            ForSupplier = forSupplier;
            RowVersion = rowVersion;
            CurrentUserOid = currentUserOid;
        }
        public int ModeId { get; }
        public string Title { get; }
        public bool ForSupplier { get; }
        public string RowVersion { get; }
        public Guid CurrentUserOid { get; }
    }
}
