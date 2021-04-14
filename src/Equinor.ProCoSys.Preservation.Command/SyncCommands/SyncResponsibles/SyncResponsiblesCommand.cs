using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.SyncCommands.SyncResponsibles
{
    public class SyncResponsiblesCommand : IRequest<Result<Unit>>
    {
    }
}
