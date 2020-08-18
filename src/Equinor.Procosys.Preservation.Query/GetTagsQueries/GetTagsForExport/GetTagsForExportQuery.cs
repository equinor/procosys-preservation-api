using System;
using Equinor.Procosys.Preservation.Domain;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetTagsQueries.GetTagsForExport
{
    public class GetTagsForExportQuery : IRequest<Result<ExportDto>>, IProjectRequest
    {
        public const SortingDirection DefaultSortingDirection = SortingDirection.Asc;
        public const SortingProperty DefaultSortingProperty = SortingProperty.Due;

        public GetTagsForExportQuery(string projectName, Sorting sorting = null, Filter filter = null)
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
