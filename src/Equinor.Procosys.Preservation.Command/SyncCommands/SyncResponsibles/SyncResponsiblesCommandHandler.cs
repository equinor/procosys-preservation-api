﻿using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Preservation.MainApi.Responsible;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.SyncCommands.SyncResponsibles
{
    public class SyncResponsiblesCommandHandler : IRequestHandler<SyncResponsiblesCommand, Result<Unit>>
    {
        private readonly IResponsibleRepository _responsibleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlantProvider _plantProvider;
        private readonly IResponsibleApiService _responsibleApiService;

        public SyncResponsiblesCommandHandler(
            IResponsibleRepository responsibleRepository,
            IUnitOfWork unitOfWork,
            IPlantProvider plantProvider, 
            IResponsibleApiService responsibleApiService)
        {
            _responsibleRepository = responsibleRepository;
            _unitOfWork = unitOfWork;
            _plantProvider = plantProvider;
            _responsibleApiService = responsibleApiService;
        }

        public async Task<Result<Unit>> Handle(SyncResponsiblesCommand request, CancellationToken cancellationToken)
        {
            var plant = _plantProvider.Plant;

            await SyncResponsibleData(plant);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<Unit>(Unit.Value);
        }

        private async Task SyncResponsibleData(string plant)
        {
            var responsibles = await _responsibleRepository.GetAllAsync();
            
            foreach (var responsible in responsibles)
            {
                var pcsResponsible = await _responsibleApiService.TryGetResponsibleAsync(plant, responsible.Code);
                if (pcsResponsible != null)
                {
                    responsible.Description = pcsResponsible.Description;
                }
            }
        }
    }
}
