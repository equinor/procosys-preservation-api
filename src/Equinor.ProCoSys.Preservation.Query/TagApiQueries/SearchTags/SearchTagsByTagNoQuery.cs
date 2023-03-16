using System.Collections.Generic;
using Equinor.ProCoSys.Common;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.TagApiQueries.SearchTags
{
    public class SearchTagsByTagNoQuery : IRequest<Result<List<PCSTagDto>>>, IProjectRequest
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
