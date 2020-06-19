using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.SyncCommands.SyncTagFunctions
{
    public class SyncTagFunctionsCommand : IRequest<Result<Unit>>
    {
    }
}
