using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.TagAggregate
{
    public class AllTagsQuery : IRequest<Result<IEnumerable<TagDto>>>
    {
    }
}
