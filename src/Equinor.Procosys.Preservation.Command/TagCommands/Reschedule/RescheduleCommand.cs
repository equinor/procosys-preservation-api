using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.Events;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.Reschedule
{
    public class RescheduleCommand : IRequest<Result<IEnumerable<IdAndRowVersion>>>, ITagCommandRequest
    {
        public RescheduleCommand(IEnumerable<IdAndRowVersion> tagIds, int weeks, RescheduledDirection direction)
        {
            Weeks = weeks;
            Direction = direction;
            Tags = tagIds ?? new List<IdAndRowVersion>();
        }

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

        public int Weeks { get; }
        public RescheduledDirection Direction { get; }
    }
}
