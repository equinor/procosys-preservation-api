using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.SyncCommands.SyncResponsibles
{
    public class SyncResponsiblesCommand : IRequest<Result<Unit>>
    {
    }
}
