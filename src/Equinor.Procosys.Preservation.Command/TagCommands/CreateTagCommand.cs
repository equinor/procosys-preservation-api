using MediatR;

namespace Equinor.Procosys.Preservation.Command.TagCommands
{
    public class CreateTagCommand : IRequest<int>
    {
        public CreateTagCommand(string tagNo, string projectNo, int journeyId, int stepId)
        {
            TagNo = tagNo;
            ProjectNo = projectNo;
            JourneyId = journeyId;
            StepId = stepId;
        }

        public string TagNo { get; private set; }
        public string ProjectNo { get; private set; }
        public int JourneyId { get; }
        public int StepId { get; private set; }
    }
}
