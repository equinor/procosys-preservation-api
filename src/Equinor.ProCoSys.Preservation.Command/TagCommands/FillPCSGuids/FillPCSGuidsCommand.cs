using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.TagCommands.FillPCSGuids
{
    public class FillPCSGuidsCommand : IRequest<Result<IEnumerable<string>>>
    {
        public FillPCSGuidsCommand(bool dryRun) => DryRun = dryRun;

        public bool DryRun { get; }
    }
}
