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
        private readonly IOptionsMonitor<AttachmentOptions> _attachmentOptions;

        public DeleteTagAttachmentCommandHandler(
            IProjectRepository projectRepository,
            IUnitOfWork unitOfWork,
            IBlobStorage blobStorage, IOptionsMonitor<AttachmentOptions> attachmentOptions)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
            _blobStorage = blobStorage;
            _attachmentOptions = attachmentOptions;
        }

        public async Task<Result<Unit>> Handle(DeleteTagAttachmentCommand request, CancellationToken cancellationToken)
        {
            var tag = await _projectRepository.GetTagByTagIdAsync(request.TagId);

            var attachment = tag.Attachments.Single(a => a.Id == request.AttachmentId);
            attachment.SetRowVersion(request.RowVersion);

            var fullBlobPath = attachment.GetFullBlobPath(_attachmentOptions.CurrentValue.BlobContainer);

            await _blobStorage.DeleteAsync(fullBlobPath, cancellationToken);
            
            tag.RemoveAttachment(attachment);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<Unit>(Unit.Value);
        }
    }
}
