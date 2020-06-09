using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.ModeCommands.CreateMode
{
    public class CreateModeCommand : IRequest<Result<int>>
    {
        public CreateModeCommand(string title, bool forSupplier, Guid currentUserOid)
        {
            Title = title;
            ForSupplier = forSupplier;
            CurrentUserOid = currentUserOid;
        }

        public string Title { get; }
        public bool ForSupplier { get; }
        public Guid CurrentUserOid { get; }
    }
}
