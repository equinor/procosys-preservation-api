using System;
using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.BulkPreserve
{
    public class BulkPreserveCommand : IRequest<Result<Unit>>
    {
        public BulkPreserveCommand(string plant, IEnumerable<int> tagIds)
        {
            Plant = plant ?? throw new ArgumentNullException(nameof(plant));
            TagIds = tagIds ?? new List<int>();
        }

        public string Plant { get; }
        public IEnumerable<int> TagIds { get; }
    }
}
