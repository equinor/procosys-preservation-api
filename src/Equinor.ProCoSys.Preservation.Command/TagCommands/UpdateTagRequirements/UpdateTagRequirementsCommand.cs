using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.TagCommands.UpdateTagRequirements
{
    public class UpdateTagRequirementsCommand : IRequest<Result<string>>, ITagCommandRequest
    {
        public UpdateTagRequirementsCommand(
            int tagId, 
            string description,
            IList<UpdateRequirementForCommand> updatedRequirements,
            IList<RequirementForCommand> newRequirements,
            IList<DeleteRequirementForCommand> deletedRequirements,
            string rowVersion)
        {
            TagId = tagId;
            Description = description;
            UpdatedRequirements = updatedRequirements ?? new List<UpdateRequirementForCommand>();
            NewRequirements = newRequirements ?? new List<RequirementForCommand>();
            DeletedRequirements = deletedRequirements ?? new List<DeleteRequirementForCommand>();
            RowVersion = rowVersion;
        }

        public int TagId { get; }
        public string Description { get; }
        public IList<UpdateRequirementForCommand> UpdatedRequirements { get; }
        public IList<RequirementForCommand> NewRequirements { get; }
        public IList<DeleteRequirementForCommand> DeletedRequirements { get; }
        public string RowVersion { get; }
    }
}
