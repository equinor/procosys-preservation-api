using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain.ProjectAccess;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.CreateTag
{
    [ProjectAccessCheck(PathToProjectType.ProjectName, nameof(ProjectName))]
    public class CreateTagCommand : IRequest<Result<List<int>>>
    {
        public CreateTagCommand(
            IEnumerable<string> tagNos,
            string projectName,
            int stepId,
            IEnumerable<Requirement> requirements,
            string remark,
            string storageArea)
        {
            TagNos = tagNos ?? new List<string>();
            ProjectName = projectName;
            StepId = stepId;
            Requirements = requirements ?? new List<Requirement>();
            Remark = remark;
            StorageArea = storageArea;
        }

        public IEnumerable<string> TagNos { get; }
        public string ProjectName { get; }
        public int StepId { get; }
        public IEnumerable<Requirement> Requirements { get; }
        public string Remark { get; }
        public string StorageArea { get; }
    }
}
