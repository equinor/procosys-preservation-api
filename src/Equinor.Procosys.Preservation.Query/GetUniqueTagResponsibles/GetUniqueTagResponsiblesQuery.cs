using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetUniqueTagResponsibles
{
    public class GetUniqueTagResponsiblesQuery : IRequest<Result<List<ResponsibleDto>>>, IProjectRequest
    {
        public GetUniqueTagResponsiblesQuery(string projectName) => ProjectName = projectName;

        public string ProjectName { get; }
    }
}
