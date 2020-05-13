using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.BlobStorage;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using Microsoft.Extensions.Options;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.ActionAttachmentCommands.Upload
{
    public class UploadActionAttachmentCommandHandler : IRequestHandler<UploadActionAttachmentCommand, Result<int>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlantProvider _plantProvider;
        private readonly IBlobStorage _blobStorage;
        private readonly IOptionsMonitor<AttachmentOptions> _attachmentOptions;

        public UploadActionAttachmentCommandHandler(
            IProjectRepository projectRepository,
            IUnitOfWork unitOfWork,
            IPlantProvider plantProvider,
            IBlobStorage blobStorage, IOptionsMonitor<AttachmentOptions> attachmentOptions)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
            _plantProvider = plantProvider;
            _blobStorage = blobStorage;
            _attachmentOptions = attachmentOptions;
        }

        public async Task<Result<int>> Handle(UploadActionAttachmentCommand request, CancellationToken cancellationToken)
        {
            var tag = await _projectRepository.GetTagByTagIdAsync(request.TagId);
            var action = tag.Actions.Single(a => a.Id == request.ActionId);

            var attachment = action.GetAttachmentByFileName(request.FileName);

            if (!request.OverwriteIfExists && attachment != null)
            {
                throw new Exception($"Tag {tag.Id} already has attachment with filename {request.FileName}");
            }

            if (attachment == null)
            {
                attachment = new ActionAttachment(
                    _plantProvider.Plant,
                    Guid.NewGuid(),
                    request.FileName);
                action.AddAttachment(attachment);
            }

            var fullBlobPath = attachment.GetFullBlobPath(_attachmentOptions.CurrentValue.BlobContainer);

            await _blobStorage.UploadAsync(fullBlobPath, request.Content, request.OverwriteIfExists, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<int>(attachment.Id);
        }
    }
}
