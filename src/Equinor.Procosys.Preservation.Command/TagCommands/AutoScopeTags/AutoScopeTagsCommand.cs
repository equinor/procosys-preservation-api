using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.AutoScopeTags
{
    public class AutoScopeTagsCommand : IRequest<Result<List<int>>>, IProjectRequest
    {
        public AutoScopeTagsCommand(
            IEnumerable<string> tagNos,
            string projectName,
            int stepId,
            string remark,
            string storageArea)
        {
            TagNos = tagNos ?? new List<string>();
            ProjectName = projectName;
            StepId = stepId;
            Remark = remark;
            StorageArea = storageArea;
        }

        public IEnumerable<string> TagNos { get; }
        public string ProjectName { get; }
        public int StepId { get; }
        public string Remark { get; }
        public string StorageArea { get; }
    }
}
