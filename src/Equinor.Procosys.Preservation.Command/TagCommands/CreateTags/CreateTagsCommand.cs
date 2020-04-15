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
