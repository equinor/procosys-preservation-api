using System;
using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetUniqueTagDisciplines
{
    public class GetUniqueTagDisciplinesQuery : IRequest<Result<List<DisciplineDto>>>
    {
        public GetUniqueTagDisciplinesQuery(string plant, string projectName)
        {
            Plant = plant ?? throw new ArgumentNullException(nameof(plant));
            ProjectName = projectName;
        }

        public string Plant { get; }
        public string ProjectName { get; }
    }
}
