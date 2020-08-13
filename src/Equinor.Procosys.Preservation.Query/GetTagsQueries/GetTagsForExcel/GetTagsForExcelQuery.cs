using System;
using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetTagsQueries.GetTagsForExcel
{
    public class GetTagsForExcelQuery : IRequest<Result<IEnumerable<TagDto>>>, IProjectRequest
    {
        public const SortingDirection DefaultSortingDirection = SortingDirection.Asc;
        public const SortingProperty DefaultSortingProperty = SortingProperty.Due;

        public GetTagsForExcelQuery(string projectName, Sorting sorting = null, Filter filter = null)
        {
            if (string.IsNullOrEmpty(projectName))
            {
                throw new ArgumentNullException(nameof(projectName));
            }
            ProjectName = projectName;
            Sorting = sorting ?? new Sorting(DefaultSortingDirection, DefaultSortingProperty);
            Filter = filter ?? new Filter();
        }

        public string ProjectName { get; }
        public Sorting Sorting { get; }
        public Filter Filter { get; }
    }
}
