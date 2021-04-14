using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.MiscCommands.Clone
{
    public class CloneCommand : IRequest<Result<Unit>>
    {
        public CloneCommand(string sourcePlant, string targetPlant)
        {
            SourcePlant = sourcePlant;
            TargetPlant = targetPlant;
        }

        public string SourcePlant { get; }
        public string TargetPlant { get; }
    }
}
