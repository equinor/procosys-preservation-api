using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.BlobStorage;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using Microsoft.Extensions.Options;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.TagAttachmentCommands.Delete
{
    public class DeleteTagAttachmentCommandHandler : IRequestHandler<DeleteTagAttachmentCommand, Result<Unit>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBlobStorage _blobStorage;
        private readonly IOptionsMonitor<BlobStorageOptions> _blobStorageOptions;

        public DeleteTagAttachmentCommandHandler(
            IProjectRepository projectRepository,
            IUnitOfWork unitOfWork,
            IBlobStorage blobStorage, IOptionsMonitor<BlobStorageOptions> blobStorageOptions)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
            _blobStorage = blobStorage;
            _blobStorageOptions = blobStorageOptions;
        }

        public async Task<Result<Unit>> Handle(DeleteTagAttachmentCommand request, CancellationToken cancellationToken)
        {
            var tag = await _projectRepository.GetTagWithAttachmentsByTagIdAsync(request.TagId);

            var attachment = tag.Attachments.Single(a => a.Id == request.AttachmentId);
            attachment.SetRowVersion(request.RowVersion);

            var fullBlobPath = attachment.GetFullBlobPath(_blobStorageOptions.CurrentValue.BlobContainer);

            await _blobStorage.DeleteAsync(fullBlobPath, cancellationToken);
            
            tag.RemoveAttachment(attachment);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<Unit>(Unit.Value);
        }
    }
}
