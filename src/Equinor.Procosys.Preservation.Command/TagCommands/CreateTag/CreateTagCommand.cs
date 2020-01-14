using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.CreateTag
{
    public class CreateTagCommand : IRequest<Result<int>>
    {
        public CreateTagCommand(string tagNumber, string projectNumber, int journeyId, int stepId, string description)
        {
            TagNumber = tagNumber;
            ProjectNumber = projectNumber;
            JourneyId = journeyId;
            StepId = stepId;
            Description = description;
        }

        public string TagNumber { get; }
        public string ProjectNumber { get; }
        public int JourneyId { get; }
        public int StepId { get; }
        public string Description { get; }
    }
}
