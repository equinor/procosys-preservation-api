using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.SyncCommands.SyncProjects
{
    public class SyncProjectsCommand : IRequest<Result<Unit>>
    {
    }
}
