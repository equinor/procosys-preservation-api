using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetAllSavedFilters
{
    public class GetAllSavedFiltersQuery : IRequest<Result<List<SavedFilterDto>>>, IProjectRequest
    {
        public GetAllSavedFiltersQuery(string projectName) => ProjectName = projectName;
        public string ProjectName { get; }
    }
}
