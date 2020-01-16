using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.AllAvailableTagsQuery
{
    public class GetAllAvailableTagsQuery : IRequest<Result<List<ProcosysTagDto>>>
    {
        public GetAllAvailableTagsQuery(string projectName, string startsWithTagNo)
        {
            ProjectName = projectName;
            StartsWithTagNo = startsWithTagNo;
        }

        public string ProjectName { get; }
        public string StartsWithTagNo { get; }
    }
}
