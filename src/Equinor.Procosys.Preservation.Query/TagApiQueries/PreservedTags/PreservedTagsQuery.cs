using System.Collections.Generic;
using Equinor.ProCoSys.Preservation.Domain;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.TagApiQueries.PreservedTags
{
    public class PreservedTagsQuery : IRequest<Result<List<ProcosysPreservedTagDto>>>, IProjectRequest
    {
        public PreservedTagsQuery(string projectName) => ProjectName = projectName;

        public string ProjectName { get; }
    }
}
