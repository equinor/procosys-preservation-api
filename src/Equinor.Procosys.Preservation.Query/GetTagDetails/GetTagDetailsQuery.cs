using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetTagDetails
{
    public class GetTagDetailsQuery : IRequest<Result<TagDetailsDto>>
    {
        public GetTagDetailsQuery(string plant, int id)
        {
            Plant = plant ?? throw new ArgumentNullException(nameof(plant));
            Id = id;
        }

        public string Plant { get; }
        public int Id { get; }
    }
}
