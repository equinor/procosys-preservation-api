using System;
using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.CreateTags
{
    public class CreateTagsCommand : IRequest<Result<List<int>>>, IProjectRequest
    {
        public CreateTagsCommand(
            IEnumerable<string> tagNos,
            string projectName,
            int stepId,
            IEnumerable<RequirementForCommand> requirements,
            string remark,
            string storageArea,
            Guid currentUserOid)
        {
            TagNos = tagNos ?? new List<string>();
            ProjectName = projectName;
            StepId = stepId;
            Requirements = requirements ?? new List<RequirementForCommand>();
            Remark = remark;
            StorageArea = storageArea;
            CurrentUserOid = currentUserOid;
        }

        public IEnumerable<string> TagNos { get; }
        public string ProjectName { get; }
        public int StepId { get; }
        public IEnumerable<RequirementForCommand> Requirements { get; }
        public string Remark { get; }
        public string StorageArea { get; }
        public Guid CurrentUserOid { get; }
    }
}
