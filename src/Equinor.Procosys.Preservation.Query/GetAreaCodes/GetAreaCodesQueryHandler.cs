using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.MainApi.AreaCode;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetAreaCodes
{
    public class GetAreaCodesQueryHandler : IRequestHandler<GetAreaCodesQuery, Result<List<AreaCodeDto>>>
    {
        private readonly IAreaCodeApiService _areaCodeApiService;
        private readonly IPlantProvider _plantProvider;

        public GetAreaCodesQueryHandler(IAreaCodeApiService areaCodeApiService, IPlantProvider plantProvider)
        {
            _areaCodeApiService = areaCodeApiService;
            _plantProvider = plantProvider;
        }

        public async Task<Result<List<AreaCodeDto>>> Handle(GetAreaCodesQuery request, CancellationToken cancellationToken)
        {
            var areaCodes = await _areaCodeApiService.GetAreaCodes(_plantProvider.Plant);
            var areaCodeDtos = areaCodes
                .Select(ac => new AreaCodeDto(ac.Code, ac.Description))
                .ToList();
            return new SuccessResult<List<AreaCodeDto>>(areaCodeDtos);
        }
    }
}
