using System.Collections.Generic;
using MediatR;

namespace Equinor.Procosys.Preservation.Query.TagAggregate
{
    public class AllTagsQuery : IRequest<IEnumerable<TagDto>>
    {
    }
}
