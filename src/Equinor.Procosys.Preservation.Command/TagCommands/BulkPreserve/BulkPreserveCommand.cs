using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.BulkPreserve
{
    public class BulkPreserveCommand : IRequest<Result<Unit>>
    {
        public BulkPreserveCommand(IEnumerable<int> tagIds) => TagIds = tagIds ?? new List<int>();

        public IEnumerable<int> TagIds { get; }
    }
}
