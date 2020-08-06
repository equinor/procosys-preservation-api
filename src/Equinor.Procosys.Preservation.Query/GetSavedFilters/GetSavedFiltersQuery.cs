using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetSavedFilters
{
    public class GetSavedFiltersQuery : IRequest<Result<List<SavedFilterDto>>>
    {
        
    }
}
