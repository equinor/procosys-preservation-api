using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetTags
{
    public class GetTagsQuery : IRequest<Result<TagsResult>>
    {
        public const SortingDirection DefaultSortingDirection = SortingDirection.Asc;
        public const SortingProperty DefaultSortingProperty = SortingProperty.Due;
        public const int DefaultPage = 0;
        public const int DefaultPagingSize = 20;

        public GetTagsQuery(string plant, string projectName, Sorting sorting = null, Filter filter = null, Paging paging = null)
        {
            if (string.IsNullOrEmpty(projectName))
            {
                throw new ArgumentNullException(nameof(projectName));
            }
            Plant = plant ?? throw new ArgumentNullException(nameof(plant));
            ProjectName = projectName;
            Sorting = sorting ?? new Sorting(DefaultSortingDirection, DefaultSortingProperty);
            Filter = filter ?? new Filter();
            Paging = paging ?? new Paging(DefaultPage, DefaultPagingSize);
        }

        public string Plant { get; }
        public string ProjectName { get; }
        public Sorting Sorting { get; }
        public Filter Filter { get; }
        public Paging Paging { get; }
    }
}
