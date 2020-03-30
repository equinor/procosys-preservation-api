using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.ModeAggregate
{
    public class GetModeByIdQuery : IRequest<Result<ModeDto>>
    {
        public GetModeByIdQuery(string plant, int id)
        {
            Plant = plant ?? throw new ArgumentNullException(nameof(plant));
            Id = id;
        }

        public string Plant { get; }
        public int Id { get; }
    }
}
