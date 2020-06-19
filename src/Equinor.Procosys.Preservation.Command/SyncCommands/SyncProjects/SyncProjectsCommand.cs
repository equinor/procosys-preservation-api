using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.SyncCommands.SyncProjects
{
    public class SyncProjectsCommand : IRequest<Result<Unit>>
    {
        public SyncProjectsCommand()
        {
        }
    }
}
