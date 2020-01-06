using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.ModeCommands.CreateMode
{
    public class CreateModeCommand : IRequest<Result<int>>
    {
        public string Title { get; set; }
    }
}
