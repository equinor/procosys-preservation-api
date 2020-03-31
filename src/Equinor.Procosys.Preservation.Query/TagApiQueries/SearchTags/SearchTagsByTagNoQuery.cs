using System;
using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.TagApiQueries.SearchTags
{
    public class SearchTagsByTagNoQuery : IRequest<Result<List<ProcosysTagDto>>>
    {
        public SearchTagsByTagNoQuery(string plant, string projectName, string startsWithTagNo)
        {
            Plant = plant ?? throw new ArgumentNullException(nameof(plant));
            ProjectName = projectName;
            StartsWithTagNo = startsWithTagNo;
        }

        public string Plant { get; }
        public string ProjectName { get; }
        public string StartsWithTagNo { get; }
    }
}
