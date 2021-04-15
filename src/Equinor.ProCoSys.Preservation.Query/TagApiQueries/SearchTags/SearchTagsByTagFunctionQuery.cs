using System.Collections.Generic;
using Equinor.ProCoSys.Preservation.Domain;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.TagApiQueries.SearchTags
{
    public class SearchTagsByTagFunctionQuery : IRequest<Result<List<PCSTagDto>>>, IProjectRequest
    {
        public SearchTagsByTagFunctionQuery(string projectName) => ProjectName = projectName;

        public string ProjectName { get; }
    }
}
