using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.TagCommands.CompletePreservation
{
    public class CompletePreservationCommand : IRequest<Result<IEnumerable<IdAndRowVersion>>>, ITagCommandRequest
    {
        public CompletePreservationCommand(IEnumerable<IdAndRowVersion> tags)
            => Tags = tags ?? new List<IdAndRowVersion>();

        public IEnumerable<IdAndRowVersion> Tags { get; }

        public int TagId
        {
            get
            {
                if (!Tags.Any())
                {
                    throw new Exception($"At least 1 {nameof(Tags)} must be given!");
                }

                return Tags.First().Id;
            }
        }
    }
}
