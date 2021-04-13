using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.SyncCommands.SyncTagFunctions
{
    public class SyncTagFunctionsCommand : IRequest<Result<Unit>>
    {
    }
}
