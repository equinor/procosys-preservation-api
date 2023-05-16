using System;
using Equinor.ProCoSys.Common;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetTagsQueries.GetTagsForExport
{
    public class GetTagsForExportQuery : IRequest<Result<ExportDto>>, IProjectRequest
    {
        public const SortingDirection DefaultSortingDirection = SortingDirection.Asc;
        public const SortingProperty DefaultSortingProperty = SortingProperty.Due;

        public GetTagsForExportQuery(
            string projectName,
            HistoryExportMode historyExportMode = HistoryExportMode.ExportNone,
            Sorting sorting = null,
            Filter filter = null)
        {
            if (string.IsNullOrEmpty(projectName))
            {
                throw new ArgumentNullException(nameof(projectName));
            }
            ProjectName = projectName;
            HistoryExportMode = historyExportMode;
            Sorting = sorting ?? new Sorting(DefaultSortingDirection, DefaultSortingProperty);
            Filter = filter ?? new Filter();
        }

        public string ProjectName { get; }
        public HistoryExportMode HistoryExportMode { get; }
        public Sorting Sorting { get; }
        public Filter Filter { get; }
    }
}
