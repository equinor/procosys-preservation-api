using System;
using Equinor.ProCoSys.Common;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetTagsQueries.GetTags
{
    public class GetTagsQuery : IRequest<Result<TagsResult>>, IProjectRequest
    {
        public const SortingDirection DefaultSortingDirection = SortingDirection.Asc;
        public const SortingProperty DefaultSortingProperty = SortingProperty.Due;
        public const int DefaultPage = 0;
        public const int DefaultPagingSize = 20;

        public GetTagsQuery(string projectName, Sorting sorting = null, Filter filter = null, Paging paging = null)
        {
            if (string.IsNullOrEmpty(projectName))
            {
                throw new ArgumentNullException(nameof(projectName));
            }
            ProjectName = projectName;
            Sorting = sorting ?? new Sorting(DefaultSortingDirection, DefaultSortingProperty);
            Filter = filter ?? new Filter();
            Paging = paging ?? new Paging(DefaultPage, DefaultPagingSize);
        }

        public string ProjectName { get; }
        public Sorting Sorting { get; }
        public Filter Filter { get; }
        public Paging Paging { get; }
    }
}
