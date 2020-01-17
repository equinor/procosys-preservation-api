using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.TagApiQueries.SearchTags
{
    public class SearchTagsQuery : IRequest<Result<List<ProcosysTagDto>>>
    {
        public SearchTagsQuery(string projectName, string startsWithTagNo)
        {
            ProjectName = projectName;
            StartsWithTagNo = startsWithTagNo;
        }

        public string ProjectName { get; }
        public string StartsWithTagNo { get; }
    }
}
