using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.StopPreservation
{
    public class StopPreservationCommand : IRequest<Result<Unit>>, ITagCommandRequest
    {
        public StopPreservationCommand(IEnumerable<int> tagIds)
            => TagIds = tagIds ?? new List<int>();

        public IEnumerable<int> TagIds { get; }
        
        public int TagId
        {
            get
            {
                if (!TagIds.Any())
                {
                    throw new Exception($"At least 1 {nameof(TagIds)} must be given!");
                }

                return TagIds.First();
            }
        }
    }
}
