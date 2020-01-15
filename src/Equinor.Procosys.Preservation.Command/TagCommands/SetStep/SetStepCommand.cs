using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.SetStep
{
    public class SetStepCommand : IRequest<Result<Unit>>
    {
        public SetStepCommand(int tagId, int stepId)
        {
            TagId = tagId;
            StepId = stepId;
        }

        public int TagId { get; }
        public int StepId { get; }
    }
}
