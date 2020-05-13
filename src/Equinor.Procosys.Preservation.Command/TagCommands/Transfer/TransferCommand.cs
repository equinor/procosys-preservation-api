using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.Transfer
{
    public class TransferCommand : IRequest<Result<IEnumerable<IdAndRowVersion>>>
    {
        public TransferCommand(IEnumerable<IdAndRowVersion> tags) => Tags = tags ?? new List<IdAndRowVersion>();

        public IEnumerable<IdAndRowVersion> Tags { get; }
    }
}
