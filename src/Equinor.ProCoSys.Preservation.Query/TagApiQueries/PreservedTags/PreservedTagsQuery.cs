using System.Collections.Generic;
using Equinor.ProCoSys.Common;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.TagApiQueries.PreservedTags
{
    public class PreservedTagsQuery : IRequest<Result<List<PCSPreservedTagDto>>>, IProjectRequest
    {
        public PreservedTagsQuery(string projectName) => ProjectName = projectName;

        public string ProjectName { get; }
    }
}
