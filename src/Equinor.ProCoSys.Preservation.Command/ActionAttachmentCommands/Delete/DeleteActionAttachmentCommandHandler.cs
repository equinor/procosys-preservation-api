using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.BlobStorage;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using Microsoft.Extensions.Options;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.ActionAttachmentCommands.Delete
{
    public class DeleteActionAttachmentCommandHandler : IRequestHandler<DeleteActionAttachmentCommand, Result<Unit>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAzureBlobService _azureBlobService;
        private readonly IOptionsSnapshot<BlobStorageOptions> _blobStorageOptions;

        public DeleteActionAttachmentCommandHandler(
            IProjectRepository projectRepository,
            IUnitOfWork unitOfWork,
            IAzureBlobService azureBlobService,
            IOptionsSnapshot<BlobStorageOptions> blobStorageOptions)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
            _azureBlobService = azureBlobService;
            _blobStorageOptions = blobStorageOptions;
        }

        public async Task<Result<Unit>> Handle(DeleteActionAttachmentCommand request, CancellationToken cancellationToken)
        {
            var tag = await _projectRepository.GetTagWithActionsByTagIdAsync(request.TagId);
            var action = tag.Actions.Single(a => a.Id == request.ActionId);
            var attachment = action.Attachments.Single(a => a.Id == request.AttachmentId);

            attachment.SetRowVersion(request.RowVersion);

            var fullBlobPath = attachment.GetFullBlobPath();
            await _azureBlobService.DeleteAsync(
                _blobStorageOptions.Value.BlobContainer, 
                fullBlobPath, 
                cancellationToken);

            action.RemoveAttachment(attachment);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<Unit>(Unit.Value);
        }
    }
}
