using System;
using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetUniqueTagModes
{
    public class GetUniqueTagModesQuery : IRequest<Result<List<ModeDto>>>
    {
        public GetUniqueTagModesQuery(string plant, string projectName)
        {
            Plant = plant ?? throw new ArgumentNullException(nameof(plant));
            ProjectName = projectName;
        }

        public string Plant { get; }
        public string ProjectName { get; }
    }
}
