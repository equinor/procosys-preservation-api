using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.TagApiQueries.SearchTags
{
    public class SearchTagsByTagFunctionQuery : IRequest<Result<List<ProcosysTagDto>>>, IProjectRequest
    {
        public SearchTagsByTagFunctionQuery(string projectName) => ProjectName = projectName;

        public string ProjectName { get; }
    }
}
