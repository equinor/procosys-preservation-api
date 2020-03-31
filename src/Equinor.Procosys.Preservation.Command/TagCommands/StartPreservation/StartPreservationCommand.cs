using System;
using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.StartPreservation
{
    public class StartPreservationCommand : IRequest<Result<Unit>>
    {
        public StartPreservationCommand(string plant, IEnumerable<int> tagIds)
        {
            Plant = plant ?? throw new ArgumentNullException(nameof(plant));
            TagIds = tagIds ?? new List<int>();
        }

        public string Plant { get; }
        public IEnumerable<int> TagIds { get; }
    }
}
