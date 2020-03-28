using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.MainApi.Discipline;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetDisciplines
{
    public class GetDisciplinesQueryHandler : IRequestHandler<GetDisciplinesQuery, Result<List<DisciplineDto>>>
    {
        private readonly IDisciplineApiService _disciplineApiService;
        private readonly IPlantProvider _plantProvider;

        public GetDisciplinesQueryHandler(IDisciplineApiService disciplineApiService, IPlantProvider plantProvider)
        {
            _disciplineApiService = disciplineApiService;
            _plantProvider = plantProvider;
        }

        public async Task<Result<List<DisciplineDto>>> Handle(GetDisciplinesQuery request, CancellationToken cancellationToken)
        {
            var disciplines = await _disciplineApiService.GetDisciplinesAsync(_plantProvider.Plant);
            var disciplineDtos = disciplines
                .Select(ac => new DisciplineDto(ac.Code, ac.Description))
                .ToList();
            return new SuccessResult<List<DisciplineDto>>(disciplineDtos);
        }
    }
}
