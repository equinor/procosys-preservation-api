using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.TagCommands.FillPCSGuids
{
    public class FillPCSGuidsCommand : IRequest<Result<Unit>>
    {
        public FillPCSGuidsCommand(bool dryRun) => DryRun = dryRun;

        public bool DryRun { get; }
    }
}
