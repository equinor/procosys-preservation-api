using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace Equinor.Procosys.Preservation.Query.TagAggregate
{
    public class AllTagsQuery : IRequest<IEnumerable<TagDto>>
    {
    }
}
