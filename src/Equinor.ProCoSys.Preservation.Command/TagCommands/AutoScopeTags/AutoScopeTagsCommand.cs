using System.Collections.Generic;
using Equinor.ProCoSys.Common;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.TagCommands.AutoScopeTags
{
    public class AutoScopeTagsCommand : IRequest<Result<List<int>>>, IProjectRequest
    {
        public AutoScopeTagsCommand(
            IList<string> tagNos,
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

        public IList<string> TagNos { get; }
        public string ProjectName { get; }
        public int StepId { get; }
        public string Remark { get; }
        public string StorageArea { get; }
    }
}
