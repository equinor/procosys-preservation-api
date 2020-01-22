using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.CreateTag
{
    public class CreateTagCommand : IRequest<Result<int>>
    {
        public CreateTagCommand(string tagNo, string projectName, int stepId, IEnumerable<Requirement> requirements)
        {
            TagNo = tagNo;
            ProjectName = projectName;
            StepId = stepId;
            Requirements = requirements;
        }

        public string TagNo { get; }
        public string ProjectName { get; }
        public int StepId { get; }
        public IEnumerable<Requirement> Requirements { get; }
    }
}
