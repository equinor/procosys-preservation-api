using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Domain;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.BulkPreserve
{
    public class BulkPreserveCommand : IRequest<Result<Unit>>, ITagRequest
    {
        public BulkPreserveCommand(IEnumerable<int> tagIds) => TagIds = tagIds ?? new List<int>();

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
