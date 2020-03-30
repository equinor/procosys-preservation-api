using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.ModeCommands.DeleteMode
{
    public class DeleteModeCommand : IRequest<Result<Unit>>
    {
        public DeleteModeCommand(string plant, int modeId)
        {
            Plant = plant ?? throw new ArgumentNullException(nameof(plant));
            ModeId = modeId;
        }

        public string Plant { get; }
        public int ModeId { get; }
    }
}
