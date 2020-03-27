using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetUniqueTagDisciplines
{
    public class GetUniqueTagDisciplinesQuery : IRequest<Result<List<DisciplineDto>>>
    {
        public GetUniqueTagDisciplinesQuery(string projectName) => ProjectName = projectName;

        public string ProjectName { get; }
    }
}
