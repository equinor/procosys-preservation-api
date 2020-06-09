using System;
using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.Transfer
{
    public class TransferCommand : IRequest<Result<IEnumerable<IdAndRowVersion>>>
    {
        public TransferCommand(IEnumerable<IdAndRowVersion> tags, Guid currentUserOid)
        {
            Tags = tags ?? new List<IdAndRowVersion>();
            CurrentUserOid = currentUserOid;
        }

        public IEnumerable<IdAndRowVersion> Tags { get; }
        public Guid CurrentUserOid { get; }
    }
}
