﻿using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.ModeCommands.UpdateMode
{
    public class UpdateModeCommandHandler : IRequestHandler<UpdateModeCommand, Result<string>>
    {
        private readonly IModeRepository _modeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateModeCommandHandler(IModeRepository modeRepository, IUnitOfWork unitOfWork)
        {
            _modeRepository = modeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(UpdateModeCommand request, CancellationToken cancellationToken)
        {
            var mode = await _modeRepository.GetByIdAsync(request.ModeId);

            mode.Title = request.Title;
            mode.ForSupplier = request.ForSupplier;
            mode.SetRowVersion(request.RowVersion);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<string>(mode.RowVersion.ConvertToString());
        }
    }
}
