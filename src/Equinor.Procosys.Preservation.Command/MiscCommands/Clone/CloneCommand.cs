using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.MiscCommands.Clone
{
    public class CloneCommand : IRequest<Result<Unit>>
    {
        public CloneCommand(string sourcePlant) => SourcePlant = sourcePlant;

        public string SourcePlant { get; }
    }
}
