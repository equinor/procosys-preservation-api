using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.Transfer
{
    public class TransferCommand : IRequest<Result<IEnumerable<IdAndRowVersion>>>, ITagCommandRequest
    {
        public TransferCommand(IEnumerable<IdAndRowVersion> tags) => Tags = tags ?? new List<IdAndRowVersion>();

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
