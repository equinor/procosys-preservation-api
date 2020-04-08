using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.TagApiQueries.SearchTags
{
    public class SearchTagsByTagNoQuery : IRequest<Result<List<ProcosysTagDto>>>, IProjectRequest
    {
        public SearchTagsByTagNoQuery(string projectName, string startsWithTagNo)
        {
            ProjectName = projectName;
            StartsWithTagNo = startsWithTagNo;
        }

        public string ProjectName { get; }
        public string StartsWithTagNo { get; }
    }
}
