using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.UpdateTagStepAndRequirements
{
    public class UpdateTagStepAndRequirementsCommand : IRequest<Result<string>>, ITagCommandRequest
    {
        public UpdateTagStepAndRequirementsCommand(
            int tagId, 
            string description,
            int stepId,
            IList<UpdateRequirementForCommand> updateRequirements,
            IList<RequirementForCommand> newRequirements,
            IList<DeleteRequirementForCommand> deleteRequirements,
            string rowVersion)
        {
            TagId = tagId;
            Description = description;
            StepId = stepId;
            UpdateRequirements = updateRequirements ?? new List<UpdateRequirementForCommand>();
            NewRequirements = newRequirements ?? new List<RequirementForCommand>();
            DeleteRequirements = deleteRequirements ?? new List<DeleteRequirementForCommand>();
            RowVersion = rowVersion;
        }

        public int TagId { get; }
        public string Description { get; }
        public int StepId { get; }
        public IList<UpdateRequirementForCommand> UpdateRequirements { get; }
        public IList<RequirementForCommand> NewRequirements { get; }
        public IList<DeleteRequirementForCommand> DeleteRequirements { get; }
        public string RowVersion { get; }
    }
}
