using System;
using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetTagRequirements
{
    public class GetTagRequirementsQuery : IRequest<Result<List<RequirementDto>>>
    {
        public GetTagRequirementsQuery(string plant, int id)
        {
            Plant = plant ?? throw new ArgumentNullException(nameof(plant));
            Id = id;
        }

        public string Plant { get; }
        public int Id { get; }
    }
}
