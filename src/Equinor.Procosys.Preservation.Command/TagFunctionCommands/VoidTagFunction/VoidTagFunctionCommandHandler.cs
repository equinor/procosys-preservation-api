using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagFunctionCommands.VoidTagFunction
{
    public class VoidTagFunctionCommandHandler : IRequestHandler<VoidTagFunctionCommand, Result<string>>
    {
        private readonly ITagFunctionRepository _tagFunctionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public VoidTagFunctionCommandHandler(ITagFunctionRepository tagFunctionRepository, IUnitOfWork unitOfWork)
        {
            _tagFunctionRepository = tagFunctionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(VoidTagFunctionCommand request, CancellationToken cancellationToken)
        {
            var tagFunction = await _tagFunctionRepository.GetByCodesAsync(request.TagFunctionCode, request.RegisterCode);

            tagFunction.Void();
            tagFunction.SetRowVersion(request.RowVersion);
            await _unitOfWork.SaveChangesAsync(request.CurrentUserOid, cancellationToken);

            return new SuccessResult<string>(tagFunction.RowVersion.ConvertToString());
        }
    }
}
