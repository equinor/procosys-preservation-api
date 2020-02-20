using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.Transfer
{
    public class TransferCommand : IRequest<Result<Unit>>
    {
        public TransferCommand(IEnumerable<int> tagIds) => TagIds = tagIds ?? new List<int>();

        public IEnumerable<int> TagIds { get; }
    }
}
