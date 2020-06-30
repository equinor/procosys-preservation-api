using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.UpdateTagStepAndRequirements
{
    public class UpdateTagStepAndRequirementsCommand : IRequest<Result<string>>, ITagCommandRequest
    {
        public UpdateTagStepAndRequirementsCommand(
            int tagId,
            int stepId,
            IList<UpdateRequirementForCommand> updatedRequirements,
            IList<RequirementForCommand> newRequirements,
            string rowVersion)
        {
            TagId = tagId;
            StepId = stepId;
            UpdatedRequirements = updatedRequirements;
            NewRequirements = newRequirements;
            RowVersion = rowVersion;
        }

        public int TagId { get; }
        public int StepId { get; }
        public IList<UpdateRequirementForCommand> UpdatedRequirements { get; }
        public IList<RequirementForCommand> NewRequirements { get; }
        public string RowVersion { get; }
    }
}
