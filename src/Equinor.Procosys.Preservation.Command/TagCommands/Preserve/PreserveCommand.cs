using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.Preserve
{
    public class PreserveCommand : IRequest<Result<Unit>>
    {
        public PreserveCommand(IEnumerable<int> tagIds) => TagIds = tagIds ?? new List<int>();

        public IEnumerable<int> TagIds { get; }
    }
}
