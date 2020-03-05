using System;
using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetTags
{
    public class GetTagsQuery : IRequest<Result<IEnumerable<TagDto>>>
    {
        public GetTagsQuery(Sorting sorting, Filter filter, Paging paging)
        {
            Sorting = sorting ?? throw new ArgumentNullException(nameof(sorting));
            Filter = filter ?? throw new ArgumentNullException(nameof(filter));
            Paging = paging ?? throw new ArgumentNullException(nameof(paging));
        }

        public Sorting Sorting { get; }
        public Filter Filter { get; }
        public Paging Paging { get; }
    }
}
