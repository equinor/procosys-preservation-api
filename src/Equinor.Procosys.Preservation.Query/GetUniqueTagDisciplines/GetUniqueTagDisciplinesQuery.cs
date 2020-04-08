using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetUniqueTagDisciplines
{
    public class GetUniqueTagDisciplinesQuery : IRequest<Result<List<DisciplineDto>>>, IProjectRequest
    {
        public GetUniqueTagDisciplinesQuery(string projectName) => ProjectName = projectName;

        public string ProjectName { get; }
    }
}
