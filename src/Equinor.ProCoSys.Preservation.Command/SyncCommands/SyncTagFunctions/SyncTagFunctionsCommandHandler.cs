using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using Equinor.ProCoSys.Preservation.MainApi.TagFunction;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.SyncCommands.SyncTagFunctions
{
    public class SyncTagFunctionsCommandHandler : IRequestHandler<SyncTagFunctionsCommand, Result<Unit>>
    {
        private readonly ITagFunctionRepository _tagFunctionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlantProvider _plantProvider;
        private readonly ITagFunctionApiService _tagFunctionApiService;

        public SyncTagFunctionsCommandHandler(
            ITagFunctionRepository tagFunctionRepository,
            IUnitOfWork unitOfWork,
            IPlantProvider plantProvider, 
            ITagFunctionApiService tagFunctionApiService)
        {
            _tagFunctionRepository = tagFunctionRepository;
            _unitOfWork = unitOfWork;
            _plantProvider = plantProvider;
            _tagFunctionApiService = tagFunctionApiService;
        }

        public async Task<Result<Unit>> Handle(SyncTagFunctionsCommand request, CancellationToken cancellationToken)
        {
            var plant = _plantProvider.Plant;

            await SyncTagFunctionData(plant);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<Unit>(Unit.Value);
        }

        private async Task SyncTagFunctionData(string plant)
        {
            var tagFunctions = await _tagFunctionRepository.GetAllAsync();
            
            foreach (var tagFunction in tagFunctions)
            {
                var pcsTagFunction = await _tagFunctionApiService.TryGetTagFunctionAsync(plant, tagFunction.Code, tagFunction.RegisterCode);
                if (pcsTagFunction != null)
                {
                    tagFunction.Description = pcsTagFunction.Description;
                }
            }
        }
    }
}
