using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.RequirementTypeAggregate
{
    public class GetRequirementTypeByIdQuery : IRequest<Result<RequirementTypeDto>>
    {
        public GetRequirementTypeByIdQuery(string plant, int id)
        {
            Plant = plant ?? throw new ArgumentNullException(nameof(plant));
            Id = id;
        }

        public string Plant { get; }
        public int Id { get; }
    }
}
