using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetDisciplines
{
    public class GetDisciplinesQuery : IRequest<Result<List<DisciplineDto>>>
    {
    }
}
