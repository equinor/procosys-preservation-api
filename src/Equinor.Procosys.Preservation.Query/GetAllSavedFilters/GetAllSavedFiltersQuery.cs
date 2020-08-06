using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetAllSavedFilters
{
    public class GetAllSavedFiltersQuery : IRequest<Result<List<SavedFilterDto>>>
    {
    }
}
