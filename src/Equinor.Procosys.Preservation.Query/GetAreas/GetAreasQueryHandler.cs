using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.MainApi.Area;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetAreas
{
    public class GetAreasQueryHandler : IRequestHandler<GetAreasQuery, Result<List<AreaDto>>>
    {
        private readonly IAreaApiService _areaApiService;
        private readonly IPlantProvider _plantProvider;

        public GetAreasQueryHandler(IAreaApiService areaApiService, IPlantProvider plantProvider)
        {
            _areaApiService = areaApiService;
            _plantProvider = plantProvider;
        }

        public async Task<Result<List<AreaDto>>> Handle(GetAreasQuery request, CancellationToken cancellationToken)
        {
            var areas = await _areaApiService.GetAreasAsync(_plantProvider.Plant);
            var areaCodes = areas
                .Select(ac => new AreaDto(ac.Code, ac.Description))
                .ToList();
            return new SuccessResult<List<AreaDto>>(areaCodes);
        }
    }
}
