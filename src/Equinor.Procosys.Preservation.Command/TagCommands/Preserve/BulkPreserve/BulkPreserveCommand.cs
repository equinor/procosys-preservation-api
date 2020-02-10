using System.Collections.Generic;

namespace Equinor.Procosys.Preservation.Command.TagCommands.Preserve.BulkPreserve
{
    public class BulkPreserveCommand : PreserveCommand
    {
        public BulkPreserveCommand(IEnumerable<int> tagIds) : base(tagIds)
        {
        }
    }
}
