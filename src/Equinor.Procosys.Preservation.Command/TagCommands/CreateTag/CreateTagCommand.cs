using MediatR;

namespace Equinor.Procosys.Preservation.Command.TagCommands.CreateTag
{
    public class CreateTagCommand : IRequest<int>
    {
        public CreateTagCommand(string tagNo, string projectNo, int journeyId, int stepId, string description)
        {
            TagNo = tagNo;
            ProjectNo = projectNo;
            JourneyId = journeyId;
            StepId = stepId;
            Description = description;
        }

        public string TagNo { get; }
        public string ProjectNo { get; }
        public int JourneyId { get; }
        public int StepId { get; }
        public string Description { get; }
    }
}
