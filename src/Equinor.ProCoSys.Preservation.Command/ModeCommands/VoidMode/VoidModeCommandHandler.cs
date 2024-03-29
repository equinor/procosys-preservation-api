﻿using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.ModeCommands.VoidMode
{
    public class VoidModeCommandHandler : IRequestHandler<VoidModeCommand, Result<string>>
    {
        private readonly IModeRepository _modeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public VoidModeCommandHandler(IModeRepository modeRepository, IUnitOfWork unitOfWork)
        {
            _modeRepository = modeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(VoidModeCommand request, CancellationToken cancellationToken)
        {
            var mode = await _modeRepository.GetByIdAsync(request.ModeId);

            mode.IsVoided = true;
            mode.SetRowVersion(request.RowVersion);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<string>(mode.RowVersion.ConvertToString());
        }
    }
}
