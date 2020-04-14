using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetTagActions
{
    public class GetTagActionsQuery : IRequest<Result<List<ActionDto>>>, ITagRequest
    {
        public GetTagActionsQuery(int tagId) => TagId = tagId;

        public int TagId { get; }
    }
}
