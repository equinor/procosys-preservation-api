using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.BulkPreserve
{
    public class BulkPreserveCommand : IRequest<Result<Unit>>, ITagCommandRequest
    {
        public BulkPreserveCommand(IEnumerable<int> tagIds, Guid currentUserOid)
        {
            TagIds = tagIds ?? new List<int>();
            CurrentUserOid = currentUserOid;
        }

        public IEnumerable<int> TagIds { get; }
        public Guid CurrentUserOid { get; }

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
