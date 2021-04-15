using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetSavedFiltersInProject
{
    public class GetSavedFiltersInProjectQuery : IRequest<Result<List<SavedFilterDto>>> // not necessary to secure SavedFilters on Project
    {
        public GetSavedFiltersInProjectQuery(string projectName) => ProjectName = projectName;
        public string ProjectName { get; }
    }
}
