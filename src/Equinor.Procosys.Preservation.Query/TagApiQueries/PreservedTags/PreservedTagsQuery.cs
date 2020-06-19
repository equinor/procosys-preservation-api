using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.TagApiQueries.PreservedTags
{
    public class PreservedTagsQuery : IRequest<Result<List<ProcosysPreservedTagDto>>>, IProjectRequest
    {
        public PreservedTagsQuery(string projectName) => ProjectName = projectName;

        public string ProjectName { get; }
    }
}
