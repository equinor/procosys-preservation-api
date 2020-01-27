using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.ProjectAggregate
{
    public class GetAllTagsInProjectQuery : IRequest<Result<IEnumerable<TagDto>>>
    {
        public GetAllTagsInProjectQuery(string projectName) => ProjectName = projectName;

        public string ProjectName { get; }
    }
}
