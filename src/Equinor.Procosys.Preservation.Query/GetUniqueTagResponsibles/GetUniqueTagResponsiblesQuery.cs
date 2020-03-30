using System;
using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetUniqueTagResponsibles
{
    public class GetUniqueTagResponsiblesQuery : IRequest<Result<List<ResponsibleDto>>>
    {
        public GetUniqueTagResponsiblesQuery(string plant, string projectName)
        {
            Plant = plant ?? throw new ArgumentNullException(nameof(plant));
            ProjectName = projectName;
        }

        public string Plant { get; }
        public string ProjectName { get; }
    }
}
