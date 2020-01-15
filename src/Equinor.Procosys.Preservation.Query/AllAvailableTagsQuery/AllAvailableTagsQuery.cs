using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.AllAvailableTagsQuery
{
    public class AllAvailableTagsQuery : IRequest<Result<List<ProcosysTagDto>>>
    {
        public AllAvailableTagsQuery(string projectName, string startsWithTagNo)
        {
            ProjectName = projectName;
            StartsWithTagNo = startsWithTagNo;
        }

        public string ProjectName { get; }
        public string StartsWithTagNo { get; }
    }
}
