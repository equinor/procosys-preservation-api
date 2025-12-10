using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.TagCommands.UndoStartPreservation
{
    public class UndoStartPreservationCommand : IRequest<Result<IEnumerable<IdAndRowVersion>>>, ITagCommandRequest
    {
        public UndoStartPreservationCommand(IEnumerable<IdAndRowVersion> tagIds)
            => Tags = tagIds ?? new List<IdAndRowVersion>();

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
