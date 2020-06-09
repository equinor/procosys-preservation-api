using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.MiscCommands.Clone
{
    public class CloneCommand : IRequest<Result<Unit>>
    {
        public CloneCommand(string sourcePlant, string targetPlant, Guid currentUserOid)
        {
            SourcePlant = sourcePlant;
            TargetPlant = targetPlant;
            CurrentUserOid = currentUserOid;
        }

        public string SourcePlant { get; }
        public string TargetPlant { get; }
        public Guid CurrentUserOid { get; }
    }
}
