using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetSavedFiltersInProject
{
    public class GetSavedFiltersInProjectQuery : IRequest<Result<List<SavedFilterDto>>>, IProjectRequest
    {
        public GetSavedFiltersInProjectQuery(string projectName) => ProjectName = projectName;
        public string ProjectName { get; }
    }
}
