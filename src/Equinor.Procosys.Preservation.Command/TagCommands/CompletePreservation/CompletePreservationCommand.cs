using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Command.TagCommands.Transfer;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.CompletePreservation
{
    public class CompletePreservationCommand : IRequest<Result<IEnumerable<IdAndRowVersion>>>, ITagCommandRequest
    {
        public CompletePreservationCommand(IEnumerable<IdAndRowVersion> tags, Guid currentUserOid)
        {
            Tags = tags ?? new List<IdAndRowVersion>();
            CurrentUserOid = currentUserOid;
        }

        public IEnumerable<IdAndRowVersion> Tags { get; }
        public Guid CurrentUserOid { get; }

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
