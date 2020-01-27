using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.Preserve
{
    public class PreserveCommand : IRequest<Result<Unit>>
    {
        public PreserveCommand(IEnumerable<int> tagIds, bool bulkPreserved)
        {
            TagIds = tagIds ?? new List<int>();
            BulkPreserved = bulkPreserved;
        }

        public IEnumerable<int> TagIds { get; }
        public bool BulkPreserved { get; }
    }
}
